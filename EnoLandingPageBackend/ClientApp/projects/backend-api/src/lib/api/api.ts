export * from './account.service';
import { AccountService } from './account.service';
export * from './account.serviceInterface'
export * from './admin.service';
import { AdminService } from './admin.service';
export * from './admin.serviceInterface'
export * from './data.service';
import { DataService } from './data.service';
export * from './data.serviceInterface'
export * from './enoEngine.service';
import { EnoEngineService } from './enoEngine.service';
export * from './enoEngine.serviceInterface'
export * from './vulnbox.service';
import { VulnboxService } from './vulnbox.service';
export * from './vulnbox.serviceInterface'
export const APIS = [AccountService, AdminService, DataService, EnoEngineService, VulnboxService];
