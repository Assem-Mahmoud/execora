/**
 * Activity status values
 */
export enum ActivityStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  ReadyForInspection = 'ReadyForInspection',
  Approved = 'Approved',
  Rework = 'Rework',
  Completed = 'Completed',
  OnHold = 'OnHold',
  Locked = 'Locked'
}

/**
 * Represents a work activity in a project
 */
export interface Activity {
  id: string;
  projectId: string;
  parentActivityId?: string | null;
  tradeId?: string | null;
  activityCode?: string | null;
  name: string;
  description?: string | null;
  plannedStartDate?: string | null;
  plannedEndDate?: string | null;
  actualStartDate?: string | null;
  actualEndDate?: string | null;
  progressPercentage: number;
  status: ActivityStatus;
  isBlocked: boolean;
  blockedById?: string | null;
  plannedQuantity?: number | null;
  unit?: string | null;
  location?: string | null;
  bimElementIds?: any;
  assignedTo?: string | null;
  createdAt: string;
  updatedAt: string;
  children?: Activity[];
}

/**
 * Progress snapshot for an activity
 */
export interface ActivityProgress {
  id: string;
  activityId: string;
  progressPercentage: number;
  reportedBy: string;
  reportedAt: string;
  notes?: string | null;
}
