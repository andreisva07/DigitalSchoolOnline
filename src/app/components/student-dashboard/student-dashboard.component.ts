import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { ToasterService } from 'src/app/services/toaster.service';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { TeacherService } from 'src/app/services/teacher.service';

@Component({
  selector: 'app-student-dashboard',
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.scss']
})
export class StudentDashboardComponent implements OnInit {
  studentClass: any;
  selectedSubject: any;
  gradesBySemester: any[] = [];
  attendance: any[] = [];
  filteredAttendance: any[] = [];
  showGradesModal: boolean = false;
  showAttendanceModal: boolean = false;
  currentUser: any;
  studentId!: number;
  selectedSemesterId: number | null = null;
  semesters: any[] = [];
  noAbsencesMessage: string = '';

  constructor(
    private authService: AuthService,
    private api: ApiService,
    private toastr: ToasterService,
    private teacherService: TeacherService
  ) { }

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    const studentCNP = this.currentUser.CNP;
    this.api.getStudentIdByCNP(studentCNP).subscribe(
      (studentId: number) => {
        this.studentId = studentId;
        this.api.getStudentClassAndSubjectsForStudent(studentId).subscribe(
          (studentClass: any) => {
            this.studentClass = studentClass;
          },
          error => {
            console.error('Error fetching student class and subjects:', error);
          }
        );
        console.log('Student ID:', studentId);
        this.fetchSemesters();
      },
      error => {
        console.error('Error fetching student ID:', error);
      }
    );
  }

  fetchSemesters() {
    this.api.getAllSemesters().subscribe(
      (semesters: any[]) => {
        this.semesters = semesters;
      },
      error => {
        console.error('Error fetching semesters:', error);
      }
    );
  }

  onSubjectSelect(subject: any) {
    this.selectedSubject = subject;
    this.showGradesAndAttendance();
  }

  showGradesAndAttendance() {
    if (!this.selectedSubject || !this.studentId) return;

    const subjectId = this.selectedSubject.id;

    const gradeRequests = this.semesters.map(semester =>
      this.teacherService.getGradesByStudentAndSubjectAndSemester(this.studentId, subjectId, semester.id)
        .pipe(
          map(grades => ({ semesterId: semester.id, grades }))
        )
    );

    forkJoin(gradeRequests).subscribe(
      (gradesBySemester: any[]) => {
        this.gradesBySemester = gradesBySemester;
        this.showGradesModal = true;
      },
      error => {
        console.error('Error fetching student grades by semester:', error);
      }
    );
  }

  openAttendanceModal() {
    this.showAttendanceModal = true;
  }

  closeGradesModal() {
    this.showGradesModal = false;
  }

  closeAttendanceModal() {
    this.showAttendanceModal = false;
    this.filteredAttendance = [];
    this.noAbsencesMessage = '';
  }

  fetchAbsences() {
    if (!this.selectedSubject || !this.selectedSemesterId) return;

    this.filteredAttendance = [];
    this.noAbsencesMessage = '';

    const subjectId = this.selectedSubject.id;
    const semesterId = this.selectedSemesterId;

    this.teacherService.getAbsencesByStudentAndSemester(this.studentId, subjectId, semesterId).subscribe(
      (absences: any[]) => {
        if (absences && absences.length > 0) {
          this.filteredAttendance = absences.map(absence => {
            const validDate = this.isValidDate(absence.date) ? new Date(absence.date) : null;
            return { date: validDate };
          }).filter(attendance => attendance.date !== null);

          if (this.filteredAttendance.length === 0) {
            this.noAbsencesMessage = 'No valid absences for this semester';
          }
        } else {
          this.noAbsencesMessage = 'No absences for this semester';
        }
      },
      error => {
        this.noAbsencesMessage = 'No absences';
      }
    );
  }

  isValidDate(date: any): boolean {
    return date && !isNaN(Date.parse(date));
  }


  getSemesterNameById(semesterId: number): string {
    const semester = this.semesters.find(s => s.id === semesterId);
    return semester ? semester.name : 'Unknown Semester';
  }

  logout() {
    this.authService.signOut();
  }
}
