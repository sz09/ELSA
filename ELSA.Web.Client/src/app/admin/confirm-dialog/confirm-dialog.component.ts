import { Component, inject } from '@angular/core';
import { DialogRef } from '@ngneat/dialog';
interface Data {
  titleKey: string
}
@Component({
  selector: 'confirm-dialog',
  templateUrl: './confirm-dialog.component.html'
})
export class ConfirmDialogComponent {
  ref: DialogRef<Data, boolean> = inject(DialogRef);
}
