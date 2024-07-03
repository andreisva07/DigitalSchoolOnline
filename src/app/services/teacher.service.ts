import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class TeacherService {
    private baseUrl = 'https://localhost:7022/api/Teacher';

    constructor(private http: HttpClient) { }

    getClassDisciplines(teacherId: number): Observable<any> {
        return this.http.get(`${this.baseUrl}/${teacherId}/class-discipline`);
    }
    getClasses(teacherId: number): Observable<any> {
        return this.http.get(`${this.baseUrl}/${teacherId}/classes`);
    }

    getSubjects(teacherId: number): Observable<any> {
        return this.http.get(`${this.baseUrl}/${teacherId}/subjects`);
    }

    addGrade(grade: any): Observable<any> {
        return this.http.post(`${this.baseUrl}/add-grade`, grade);
    }

    addAttendance(attendance: any): Observable<any> {
        return this.http.post(`${this.baseUrl}/add-attendance`, attendance);
    }

    getTeacherId(cnp: string): Observable<number> {
        return this.http.get<number>(`${this.baseUrl}/current-teacher-id?cnp=${cnp}`);
    }
    updateGrade(grade: any) {
        return this.http.post(`${this.baseUrl}/update-grade`, grade);
    }
    deleteGrade(gradeId: number): Observable<any> {
        return this.http.delete(`${this.baseUrl}/delete-grade/${gradeId}`);
    }
    getAbsencesByStudentAndSemester(studentId: number, subjectId: number, semesterId: number): Observable<any[]> {
        return this.http.get<any[]>(`${this.baseUrl}/student/${studentId}/subject/${subjectId}/semester/${semesterId}/absences`);
    }
    getGradesByStudentAndSubjectAndSemester(studentId: number, subjectId: number, semesterId: number): Observable<any[]> {
        const url = `${this.baseUrl}/grades?studentId=${studentId}&subjectId=${subjectId}&semesterId=${semesterId}`;
        return this.http.get<any[]>(url);
    }
}

