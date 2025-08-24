import { Component } from '@angular/core';
import { LogView } from '../../models/log-view';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LogsviewService } from '../../services/logsview.service';

@Component({
  selector: 'app-logs-view',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './logs-view.component.html',
  styleUrl: './logs-view.component.css'
})
export class LogsViewComponent {
  lines: string[] = [];
  last = 200;
  level = '';
  q = '';
  rows: LogView[] = [];
  loading = false;

  constructor(private logs: LogsviewService) { }

  ngOnInit(): void {
    this.reload();
  }

  reload(): void {
    this.loading = true;
    this.logs.get(this.last, this.level || undefined, this.q || undefined)
      .subscribe({
        next: rows => { this.rows = rows; this.loading = false; },
        error: _ => { this.rows = []; this.loading = false; }
      });
  }

  badgeClass(level?: string) {
    const l = (level ?? '').toUpperCase();
    return l === 'ERR' ? 'bg-danger'
      : l === 'WRN' ? 'bg-warning text-dark'
        : 'bg-secondary';
  }

  trackByIdx = (_: number, __: LogView) => _;
}
