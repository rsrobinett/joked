import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'highlight'
})
export class HighlightPipe implements PipeTransform {

  transform(value: string, args: any): any {
    if (!args) {return value;}
    var re = new RegExp(args, 'gi'); //g=all, i=case insensitive
    return value.replace(re, "<mark>$&</mark>");
}

}
