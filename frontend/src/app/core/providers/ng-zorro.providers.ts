import { Provider } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzInputModule } from 'ng-zorro-antd/input';

export const ngZorroProviders: Provider[] = [
  NzMessageService,
  NzCardModule,
  NzButtonModule,
  NzIconModule,
  NzAlertModule,
  NzSpinModule,
  NzInputModule
];