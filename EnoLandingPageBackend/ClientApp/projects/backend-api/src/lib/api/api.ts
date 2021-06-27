export * from './account.service';
import { AccountService } from './account.service';
export * from './account.serviceInterface'
export * from './admin.service';
import { AdminService } from './admin.service';
export * from './admin.serviceInterface'
export * from './data.service';
import { DataService } from './data.service';
export * from './data.serviceInterface'
export * from './scoreboardInfo.service';
import { ScoreboardInfoService } from './scoreboardInfo.service';
export * from './scoreboardInfo.serviceInterface'
export * from './vulnbox.service';
import { VulnboxService } from './vulnbox.service';
export * from './vulnbox.serviceInterface'
export const APIS = [AccountService, AdminService, DataService, ScoreboardInfoService, VulnboxService];
