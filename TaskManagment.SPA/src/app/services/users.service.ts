import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { CreateUserDto } from '../models/create-user-dto.dto';
import { UpdateUserDto } from '../models/update-user-dto.dto';
import { UserDto } from '../models/user-dto.dto';

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(private http: HttpClient) { }
  list() { return this.http.get<UserDto[]>(`${environment.api}/users/GetAll`); }
  get(id: string) { return this.http.get<UserDto>(`${environment.api}/users/Get/${id}`); }
  create(dto: CreateUserDto) { return this.http.post<UserDto>(`${environment.api}/users/Create`, dto); }
  update(id: string, dto: UpdateUserDto) { return this.http.post<UserDto>(`${environment.api}/users/Update/${id}`, dto); }
  delete(id: string) { return this.http.post(`${environment.api}/users/Delete/${id}`, {}); }
}
