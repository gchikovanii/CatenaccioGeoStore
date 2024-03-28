import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-lang',
  templateUrl: './lang.component.html',
  styleUrl: './lang.component.scss'
})
export class LangComponent {
  lang: string ='';
  langa: string ='';
  constructor(private translateService: TranslateService){}

  toggleLanguage(event: any): void {
    const target = event.target as HTMLInputElement; 
    if (target && target.checked !== undefined) {
      const checked = target.checked;
      if (checked) {
        this.changeLang('en');
        this.langa = 'en';
      } else {
        this.changeLang('ka');
        this.langa = 'ka';

      }
    }
    const body = document.querySelector('body');
    if (body) {
      if (this.langa === 'ge') {
        body.classList.add('lang-ge');
      } else {
        body.classList.remove('lang-ge');
      }
    }

  }
  changeLang(lang:string){
    localStorage.setItem('lang',lang);
    this.translateService.use(lang);
  }

}
