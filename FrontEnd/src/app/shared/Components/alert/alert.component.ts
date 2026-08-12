import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './alert.component.html',
  styleUrl: './alert.component.scss',
})
export class AlertComponent {
  @Input() message: string = ''; // الرسالة التي سيتم عرضها
  @Input() type: 'success' | 'danger' | 'warning' | 'info' = 'info'; // نوع التنبيه
  @Input() autoDismiss: boolean = true; // إغلاق التنبيه تلقائيًا
  @Input() dismissTime: number = 3000; // الوقت قبل الإغلاق التلقائي (بالمللي ثانية)

  visible: boolean = true;

  ngOnInit(): void {
    // إغلاق التنبيه تلقائيًا بعد وقت محدد إذا كان autoDismiss مفعلاً
    if (this.autoDismiss) {
      setTimeout(() => {
        this.visible = false;
      }, this.dismissTime);
    }
  }

  // دالة لإغلاق التنبيه يدويًا
  closeAlert(): void {
    this.visible = false;
  }
}
