import { ToastType } from "./toast-type";

export interface Toast {
  text: string;
  title?: string;
  type?: ToastType;
  delay?: number;
}
