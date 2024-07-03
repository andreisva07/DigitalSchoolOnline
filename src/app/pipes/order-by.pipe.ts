import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'orderBy'
})
export class OrderByPipe implements PipeTransform {
    transform(array: any[], field: string, order: string = 'asc'): any[] {
        if (!Array.isArray(array) || array.length <= 1) {
            return array;
        }

        const sortOrder = order === 'desc' ? -1 : 1;

        return array.sort((a, b) => {
            let comparison = 0;

            if (a[field] < b[field]) {
                comparison = -1;
            } else if (a[field] > b[field]) {
                comparison = 1;
            }

            return comparison * sortOrder;
        });
    }
}
