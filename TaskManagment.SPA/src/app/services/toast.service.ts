import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { Toast } from '../models/toast';
import { ToastType } from '../models/toast-type';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  private _toasts$ = new Subject<Toast>();
  toasts$ = this._toasts$.asObservable();

  show(text: string, type: ToastType = ToastType.info, title?: string, delay = 3000) {
    this._toasts$.next({ text, type, title, delay });
  }
  success(text: string, title = 'Success') { this.show(text, ToastType.success, title); }
  info(text: string, title = 'Info') { this.show(text, ToastType.info, title); }
  warn(text: string, title = 'Warning') { this.show(text, ToastType.warning, title, 5000); }
  error(text: string, title = 'Error') { this.show(text, ToastType.danger, title, 6000); }
}
