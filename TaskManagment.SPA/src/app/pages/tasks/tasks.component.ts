import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { TasksService } from '../../services/tasks.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, NgForm, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserDto } from '../../models/user-dto.dto';
import { UsersService } from '../../services/users.service';
import { ToastService } from '../../services/toast.service';
import { TaskCreateDto } from '../../models/task-create-dto.dto';
import { TaskDto } from '../../models/task-dto.dto';
import { TaskStatus } from '../../models/task-status';



@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './tasks.component.html',
  styleUrl: './tasks.component.css'
})

export class TasksComponent implements OnInit {
  TaskStatus = TaskStatus;
  statuses = Object.values(TaskStatus).filter(v => typeof v === 'number') as TaskStatus[];

 
  tasks: TaskDto[] = [];
  users: UserDto[] = [];
  userNameById = new Map<string, string>();

  form: TaskCreateDto = { title: '', description: '', status: TaskStatus.New, assignedUserId: '' };

  isAdmin = false;
  error: string | null = null;


  editingId: number | null = null;
  editForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(1000)]],
    status: [TaskStatus.New, Validators.required],
    assignedUserId: ['', Validators.required]
  });

  constructor(
    private taskService: TasksService,
    private userService: UsersService,
    private auth: AuthService,
    private fb: FormBuilder,
    private toasts: ToastService
  ) {
    this.isAdmin = this.auth.role() === 'Admin';
  }

  ngOnInit(): void {
    this.load();

    if (this.isAdmin) {
      this.userService.list().subscribe({
        next: u => {
          this.users = u;
          this.userNameById.clear();
          u.forEach(x => this.userNameById.set(x.id, x.fullName || x.email));
        }
      });
    }
  }

  load() {
    if (this.isAdmin) {
      this.taskService.list().subscribe({
        next: t => (this.tasks = t),
        error: _ => (this.error = 'Failed to load tasks')
      });
    } else {
      this.taskService.myTasks().subscribe({
        next: t => (this.tasks = t),
        error: _ => {
          this.tasks = [];
          for (let id = 1; id <= 20; id++) {
            this.taskService.get(id).subscribe({ next: task => this.tasks.push(task) });
          }
        }
      });
    }
  }


  create(f: NgForm) {
    if (f.invalid) {
      this.toasts.warn('Please fix the validation errors.');
      return;
    }
    this.taskService.create(this.form).subscribe({
      next: _ => {
        this.form = { title: '', description: '', status: TaskStatus.New, assignedUserId: '' };
        this.toasts.success('Task created');
        f.resetForm({ status: TaskStatus.New });
        this.load();
      },
      error: _ => (this.error = 'Create failed')
    });
  }

  setStatus(t: TaskDto, status: TaskStatus) {
    this.taskService.updateStatus(t.id, status).subscribe({
      next: taskStatus => (t.status = taskStatus.status)
    });
  }

  remove(id: number) {
    if (!confirm('Delete task?')) return;
    this.taskService.delete(id).subscribe({
      next: _ => {
        this.toasts.success('Task deleted');
        this.load();
      }
    });
  }


  openEdit(t: TaskDto) {
    if (!this.isAdmin) return;
    this.editingId = t.id;
    this.editForm.setValue({
      title: t.title ?? '',
      description: t.description ?? '',
      status: t.status,
      assignedUserId: t.assignedUserId
    });
    const el = document.getElementById('editTaskModal')!;
    const modal = (window as any).bootstrap.Modal.getOrCreateInstance(el);
    modal.show();
  }

  saveEdit() {
    if (this.editForm.invalid || this.editingId == null) {
      this.toasts.warn('Please fix validation errors.');
      return;
    }
    const dto = this.editForm.getRawValue();
    this.taskService.update(this.editingId, dto).subscribe({
      next: updated => {
        const i = this.tasks.findIndex(x => x.id === updated.id);
        if (i > -1) this.tasks[i] = updated;
        this.toasts.success('Task updated');
        const el = document.getElementById('editTaskModal')!;
        const modal = (window as any).bootstrap.Modal.getOrCreateInstance(el);
        modal.show();
        this.editingId = null;
      }
    });
  }

  assignedLabel(t: TaskDto) {
    return t.assignedUserFullName
      || this.userNameById.get(t.assignedUserId)
      || t.assignedUserEmail
      || t.assignedUserId;
  }
}
