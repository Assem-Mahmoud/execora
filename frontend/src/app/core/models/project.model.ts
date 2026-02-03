/**
 * Project status values
 */
export enum ProjectStatus {
  Draft = 'Draft',
  Active = 'Active',
  OnHold = 'OnHold',
  Completed = 'Completed',
  Archived = 'Archived'
}

/**
 * Represents a construction project
 */
export interface Project {
  id: string;
  tenantId: string;
  organizationId?: string | null;
  name: string;
  code?: string | null;
  description?: string | null;
  projectStatus: ProjectStatus;
  startDate?: string | null;
  endDate?: string | null;
  location?: string | null;
  latitude?: number | null;
  longitude?: number | null;
  bimModelId?: string | null;
  bimModelVersion?: number | null;
  projectManagerId?: string | null;
  createdBy?: string | null;
  createdAt: string;
  updatedAt: string;
}
