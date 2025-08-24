export interface LogView {
  time?: string;
  level?: 'INF' | 'WRN' | 'ERR' | string;
  source?: string | null;
  path?: string | null;
  message: string;
  exception?: string | null;
  raw: string;
}
