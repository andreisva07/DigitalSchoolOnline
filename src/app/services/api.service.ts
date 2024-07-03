import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl: string = 'https://localhost:7022/api';

  constructor(private http: HttpClient) { }

  getUsers() {
    return this.http.get<any>(`${this.baseUrl}/User`);
  }

  getTeachers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher`);
  }

  getStudents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Student`);
  }

  getSubjects(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Subject`);
  }

  getAvailableClasses(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Class/available`);
  }

  addStudent(firstName: string, lastName: string, gender: number, Cnp: string, classId: number): Observable<any> {
    const studentData = { firstName, lastName, gender, Cnp, classId };
    return this.http.post(`${this.baseUrl}/Student`, studentData);
  }

  getAvailableSubjectsForTeacher(teacherId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/available-subjects?teacherId=${teacherId}`);
  }

  getAvailableTeachersForSubject(subjectId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Subject/available-teachers/${subjectId}`);
  }

  getClassById(classId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/Class/class-by-id?classId=${classId}`);
  }

  getClassId(series: string, number: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Class/class-id?series=${series}&number=${number}`);
  }

  getTeacherId(firstName: string, lastName: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Teacher/teacher-id?firstName=${firstName}&lastName=${lastName}`);
  }

  getSubjectId(subjectName: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Subject/subject-id?subjectName=${subjectName}`);
  }

  addProfessor(firstName: string, lastName: string, Cnp: string): Observable<any> {
    const professorData = { firstName, lastName, Cnp };
    return this.http.post(`${this.baseUrl}/Teacher/register`, professorData);
  }

  deleteTeacher(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Teacher/${id}`);
  }

  deleteStudent(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Student/${id}`);
  }

  addSubject(subject: { name: string }): Observable<any> {
    return this.http.post(`${this.baseUrl}/Subject`, subject);
  }

  deleteSubject(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Subject/${id}`);
  }

  addClass(series: string, number: number): Observable<any> {
    const classData = { series, number };
    return this.http.post(`${this.baseUrl}/Class`, classData);
  }

  deleteClass(id: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/Class/${id}`);
  }

  addTeacherSubject(teacherSubjectData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/TeacherSubject`, teacherSubjectData);
  }

  getLinkedSubjectsForTeacher(teacherId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/linked-subjects?teacherId=${teacherId}`);
  }

  deleteTeacherSubjectLink(teacherId: number, subjectId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/TeacherSubject/remove?teacherId=${teacherId}&subjectId=${subjectId}`);
  }
  getClassStudents(classId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Class/students?classId=${classId}`);
  }
  getStudentsWithGradesByClass(classId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Student/class/${classId}/grades`);
  }
  getAttendanceByClassAndDate(classId: number, subjectId: number, date: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Class/attendance-by-date?classId=${classId}&subjectId=${subjectId}&date=${date}`);
  }
  getGradesByStudentAndSubject(studentId: number, subjectId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Class/student/${studentId}/subject/${subjectId}/grades`);
  }

  getStudentClassAndSubjects(studentId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Student/${studentId}/class-and-subjects`);
  }
  getStudentClassAndSubjectsForStudent(studentId: number): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Student/${studentId}/class-subjects`);
  }
  getAttendanceByStudentAndSubject(studentId: number, subjectId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Student/${studentId}/subject/${subjectId}/attendance`);
  }
  getStudentIdByCNP(cnp: string): Observable<number> {
    return this.http.get<number>(`${this.baseUrl}/Student/student-id?cnp=${cnp}`);
  }
  getStudentSubjectAbsences(studentId: number, subjectId: number, month: number) {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/student-subject-absences?studentId=${studentId}&subjectId=${subjectId}&month=${month}`);
  }
  addTeacherClassLink(teacherClassData: any): Observable<any> {
    return this.http.post(`${this.baseUrl}/TeacherClassDiscipline`, teacherClassData);
  }

  deleteTeacherClassLink(teacherId: number, classId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/TeacherClass/remove?teacherId=${teacherId}&classId=${classId}`);
  }

  getLinkedClassesForTeacher(teacherId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/linked-classes?teacherId=${teacherId}`);
  }
  getAvailableClassesForTeacher(teacherId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/available-classes-for-teacher?teacherId=${teacherId}`);
  }
  getAvailableSubjectsForTeacherClass(teacherId: number, classId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/TeacherClassDiscipline/available-subjects?teacherId=${teacherId}&classId=${classId}`);
  }
  getCurrentSemester(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/Semester/current`);
  }
  getAllSemesters(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Semester/all`);
  }
  getClosedSemesters(): Observable<any> {
    return this.http.get(`${this.baseUrl}/Semester/closed`);
  }
  addSemester(semester: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Semester`, semester);
  }
  closeSemester(semesterId: number): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/Semester/${semesterId}/close`, {});
  }
  deleteSemester(semesterId: number): Observable<any> {
    return this.http.delete<any>(`${this.baseUrl}/Semester/${semesterId}/remove`, {});
  }
  reactivateSemester(semesterId: number): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/Semester/${semesterId}/reactivate`, {});
  }
  getStudentsByClass(classId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Student/class/${classId}`);
  }

  transferStudent(studentId: number, newClassId: number): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/Student/transfer`, { studentId, newClassId });
  }


  getSubjectsByTeacher(teacherId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/${teacherId}/subjects`);
  }
  getStudentsByClassWithDetails(classId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Student/class/${classId}/details`);
  }

  getAllTeachersWithDetails(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/details`);
  }


  getTeacherClassDisciplineLinks(teacherId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/Teacher/${teacherId}/class-discipline`);
  }

  deleteTeacherClassDisciplineLink(link: { teacherId: any, classId: any, subjectId: any }): Observable<any> {
    const { teacherId, classId, subjectId } = link;
    return this.http.delete(`${this.baseUrl}/TeacherClassDiscipline/remove`, { body: { teacherId, classId, subjectId } });
  }
  checkTeacherClassSubjectExists(classId: number, subjectId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/TeacherClassDiscipline/check-subject-assignment?classId=${classId}&subjectId=${subjectId}`);
  }
  updateStudent(id: number, student: any) {
    return this.http.put<any[]>(`${this.baseUrl}/Student/${id}/update`, student);
  }

}
