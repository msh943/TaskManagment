import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, catchError, of } from 'rxjs';
import { LogView } from '../models/log-view';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class LogsviewService {

  constructor(private http: HttpClient) { }
  get(last = 200, level?: string, search?: string): Observable<LogView[]> {
    let params = new HttpParams().set('last', String(last));
    if (level) params = params.set('level', level);
    if (search) params = params.set('search', search);

    return this.http
      .get<LogsResponse>(`${environment.api}/Logs/GetLogs`, { params })
      .pipe(
        map(r => r?.items ?? []),
        catchError(() => of<LogView[]>([]))
      );
  }
}
interface LogsResponse {
  items: LogView[];
}
