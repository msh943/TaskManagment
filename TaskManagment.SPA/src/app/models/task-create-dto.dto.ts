import { TaskStatus } from "./task-status";

export interface TaskCreateDto {
  title: string;
  description?: string;
  status: TaskStatus;
  assignedUserId: string;
}
