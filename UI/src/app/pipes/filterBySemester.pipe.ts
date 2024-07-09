import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'filterBySemester'
})
export class FilterBySemesterPipe implements PipeTransform {
    transform(items: any[], semesterId: number): any[] {
        if (!items || !semesterId) {
            return [];
        }
        return items.filter(item => item.semesterId === semesterId);
    }
}
