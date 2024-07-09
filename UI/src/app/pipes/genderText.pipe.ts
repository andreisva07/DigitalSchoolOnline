import { Pipe, PipeTransform } from '@angular/core';
import { Gender } from '../helpers/gender.enum';

@Pipe({
    name: 'genderText'
})
export class GenderTextPipe implements PipeTransform {
    transform(value: number): string {
        switch (value) {
            case 0:
                return 'Male';
            case 1:
                return 'Female';
            case 2:
                return 'NonBinary';
            default:
                return '';
        }
    }
}
