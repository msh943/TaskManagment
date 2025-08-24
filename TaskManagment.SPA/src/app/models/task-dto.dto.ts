import { TaskStatus } from "./task-status";

export interface TaskDto {
  id: number;
  title: string;
  description?: string;
  status: TaskStatus;
  assignedUserId: string;
  assignedUserEmail?: string;
  assignedUserFullName?: string;
}
