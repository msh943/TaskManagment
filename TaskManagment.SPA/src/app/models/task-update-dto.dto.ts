import { TaskStatus } from "./task-status";

export interface TaskUpdateDto {
  title?: string;
  description?: string;
  status?: TaskStatus;
  assignedUserId?: string;
}
