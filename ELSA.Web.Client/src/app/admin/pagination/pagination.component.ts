import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'pagination',
  templateUrl: './pagination.component.html'
})
export class PaginationComponent {
  @Output() onPageChange = new EventEmitter<number>();
  @Input() pageSize!: number;
  @Input('selectedPage')
  set setSelectedPage(selectedPage: number) {
    if (typeof(selectedPage) === undefined) {
      return
    }
    this.selectedPage = selectedPage;
    this.buildPaging();
  }
  selectedPage?: number;
  @Input('total')
  set setTotal(total: number) {
    this.total = total;
    this.buildPaging();
  }
  total!: number;

  readonly NUMBER_SHOWING_PAGE: number = 3;

  posiblePages: number[] = []
  constructor() { }

  buildPaging() {
    if (!this.total) {
      return;
    }
    var maxPage = Math.ceil(this.total / this.pageSize);
    var possiblePages: number[] = [];
    var posibleFirstPages: number[] = [];
    for (var i = 1; i <= this.NUMBER_SHOWING_PAGE; i++)
    {
      posibleFirstPages.push(i);
    }
    var posibleLastPages: number[] = [];
    for (var i = 0; i < this.NUMBER_SHOWING_PAGE; i++)
    {
      posibleLastPages.push(maxPage - i);
    }
    posibleLastPages = posibleLastPages.sort((a, b) => {
      return a - b;
    })

    if (maxPage <= this.NUMBER_SHOWING_PAGE * 2)
    {
      for (var i = 1; i <= maxPage; i++)
      {
        possiblePages.push(i);
      }
    }
    else {
      if (this.selectedPage) {
        if (this.selectedPage < 2 || this.selectedPage > maxPage - 2) {
          posibleFirstPages.forEach(d => {
            possiblePages.push(d)
          })
          posibleLastPages.forEach(d => {
            possiblePages.push(d)
          })
        }
        else {
          possiblePages.push(1);

          possiblePages.push(this.selectedPage - 1);
          possiblePages.push(this.selectedPage);
          possiblePages.push(this.selectedPage + 1);

          possiblePages.push(maxPage);
        }
      }
    }
    var saved = 0;
    this.posiblePages = [];
    possiblePages = [...new Set(possiblePages)];
    possiblePages.forEach(item =>
      {
        if (item > saved + 1) {
          this.posiblePages.push(-1);
        }
        this.posiblePages.push(item);
        saved = item;
    })
  }

  goToPage(pageNumber: any) {
    this.onPageChange.emit(pageNumber);
    this.selectedPage = pageNumber;
  }
}
