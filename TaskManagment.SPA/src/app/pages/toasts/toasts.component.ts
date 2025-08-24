import { Component, OnDestroy } from '@angular/core';
import { Subscription, timer } from 'rxjs';
import { Toast } from '../../models/toast';
import { ToastType } from '../../models/toast-type';
import { ToastService } from '../../services/toast.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-toasts',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './toasts.component.html',
  styleUrl: './toasts.component.css'
})
export class ToastsComponent implements OnDestroy {
  items: Toast[] = [];
  private sub: Subscription;


  ToastType = ToastType;

  constructor(private toasts: ToastService) {
    this.sub = this.toasts.toasts$.subscribe(t => {

      this.items.push({ ...t, type: t.type ?? ToastType.info });
      const idx = this.items.length - 1;
      const delay = t.delay ?? 3000;
      timer(delay).subscribe(() => this.items.splice(idx, 1));
    });
  }

  bgClass(t: Toast): string {
    switch (t.type) {
      case ToastType.success: return 'text-bg-success';
      case ToastType.warning: return 'text-bg-warning';
      case ToastType.danger: return 'text-bg-danger';
      default: return 'text-bg-info';
    }
  }

  close(t: Toast) { this.items = this.items.filter(x => x !== t); }
  ngOnDestroy() { this.sub.unsubscribe(); }
}
