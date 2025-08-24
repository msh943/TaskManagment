import { Component, OnInit } from '@angular/core';
import { UserDto } from '../../models/user-dto.dto';
import { AuthService } from '../../services/auth.service';
import { UsersService } from '../../services/users.service';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ToastService } from '../../services/toast.service';
import { TasksService } from '../../services/tasks.service';
import { Modal } from 'bootstrap';


declare const bootstrap: any;
@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  isAdmin = false;


  totalUsers?: number;
  totalTasks?: number;
  myTasksCount?: number;


  users: UserDto[] = [];
  loadingUsers = false;


  submitted = false;
  userForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: ['User', Validators.required]
  });

  editingUserId: string | null = null;
  editUserForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    role: ['User', Validators.required]
  });

  constructor(
    private auth: AuthService,
    private userService: UsersService,
    private taskService: TasksService,
    private fb: FormBuilder,
    private toasts: ToastService
  ) {
    this.isAdmin = this.auth.role() === 'Admin';
  }

  ngOnInit(): void {
    this.refreshStats();
    if (this.isAdmin) this.loadUsers();
    else this.loadMyTasksCount();
  }


  refreshStats() {
    if (!this.isAdmin) return;
    this.userService.list().subscribe({ next: u => (this.totalUsers = u.length) });
    this.taskService.list().subscribe({ next: t => (this.totalTasks = t.length) });
  }

  loadMyTasksCount() {

    this.taskService.myTasks().subscribe({
      next: t => (this.myTasksCount = t.length),
      error: _ => (this.myTasksCount = undefined)
    });
  }


  loadUsers() {
    this.loadingUsers = true;
    this.userService.list().subscribe({
      next: u => {
        this.users = u;
        this.loadingUsers = false;
      },
      error: _ => {
        this.loadingUsers = false;
        this.toasts.error('Failed to load users');
      }
    });
  }

  createUser() {
    this.submitted = true;
    if (this.userForm.invalid) {
      this.toasts.warn('Some Fileds Are Required!');
      return;
    }
    const dto = this.userForm.getRawValue();
    this.userService.create(dto).subscribe({
      next: _ => {
        this.toasts.success('User created.');
        this.userForm.reset({ fullName: '', email: '', password: '', role: 'User' });
        this.submitted = false;
        this.loadUsers();
        this.refreshStats();
      }
    });
  }

  openEditUser(u: UserDto) {
    this.editingUserId = u.id;
    this.editUserForm.setValue({
      fullName: u.fullName ?? '',
      email: u.email,
      role: u.role || 'User'
    });
    const el = document.getElementById('editUserModal') as HTMLElement;
    Modal.getOrCreateInstance(el).show();
  }

  saveEditUser() {
    if (this.editUserForm.invalid || !this.editingUserId) {
      this.toasts.warn('Please fix validation errors.');
      return;
    }
    const dto = this.editUserForm.getRawValue();
    this.userService.update(this.editingUserId, dto).subscribe({
      next: updated => {
        const i = this.users.findIndex(x => x.id === updated.id);
        if (i > -1) this.users[i] = updated;
        this.toasts.success('User updated.');
        const el = document.getElementById('editUserModal') as HTMLElement;
        Modal.getInstance(el)?.hide();
        this.refreshStats();
      }
    });
  }

  deleteUser(id: string) {
    if (!confirm('Delete this user?')) return;
    this.userService.delete(id).subscribe({
      next: _ => {
        this.toasts.success('User deleted.');
        this.loadUsers();
        this.refreshStats();
      }
    });
  }


  get fullName() { return this.userForm.controls.fullName; }
  get email() { return this.userForm.controls.email; }
  get password() { return this.userForm.controls.password; }
  get role() { return this.userForm.controls.role; }
}
