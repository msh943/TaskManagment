import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../services/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toasts = inject(ToastService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      const status = err.status;
      let msg = 'Request failed';
      if (err.error?.title) msg = err.error.title;
      else if (typeof err.error === 'string') msg = err.error;
      else if (err.message) msg = err.message;

      if (status === 0) toasts.error('Network error — is the API running?');
      else if (status === 401) toasts.warn('Please log in again.');
      else if (status === 403) toasts.warn('Not allowed.');
      else if (status !== 404) toasts.error(msg);
      return throwError(() => err);
    })
  );
};
