import {
  Directive,
  inject,
  Input,
  OnInit,
  TemplateRef,
  ViewContainerRef,
} from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]',
  standalone: true,
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[] = [];
  private auth = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateref = inject(TemplateRef);

  ngOnInit(): void {
    if (this.auth.roles()?.some((r) => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateref);
    } else {
      this.viewContainerRef.clear();
    }
  }
}
