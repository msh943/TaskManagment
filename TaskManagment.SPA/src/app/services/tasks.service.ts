import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { TaskCreateDto } from '../models/task-create-dto.dto';
import { TaskDto } from '../models/task-dto.dto';
import { TaskUpdateDto } from '../models/task-update-dto.dto';
import { TaskStatus } from '../models/task-status';

@Injectable({
  providedIn: 'root'
})
export class TasksService {

  constructor(private http: HttpClient) { }
  list() { return this.http.get<TaskDto[]>(`${environment.api}/tasks/GetAll`); }
  get(id: number) { return this.http.get<TaskDto>(`${environment.api}/tasks/GetById/${id}`); }
  create(dto: TaskCreateDto) { return this.http.post<TaskDto>(`${environment.api}/tasks/Create`, dto); }
  update(id: number, dto: TaskUpdateDto) { return this.http.post<TaskDto>(`${environment.api}/tasks/Update/${id}`, dto); }
  updateStatus(id: number, status: TaskStatus) { return this.http.post<TaskDto>(`${environment.api}/tasks/UpdateStatus/${id}`, { status }); }
  delete(id: number) { return this.http.post(`${environment.api}/tasks/Delete/${id}`,  {}); }
  myTasks() {
    return this.http.get<any[]>(`${environment.api}/tasks/MyTasks`);
  }
}
