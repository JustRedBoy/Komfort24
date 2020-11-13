import { ViewportScroller } from '@angular/common';
import { Component } from '@angular/core';

@Component({
    selector: 'main-page',
    styleUrls: ['./main-page.component.css'],
    templateUrl: './main-page.component.html'
})
export class MainPageComponent {
    constructor(private scroller: ViewportScroller) {}

    scrollToServicePage() {
        this.scroller.scrollToAnchor('service-page');
    }
}
