import { Component, OnInit } from '@angular/core';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { TeacherService } from 'src/app/services/teacher.service';
import { ToasterService } from 'src/app/services/toaster.service';
import { finalize, forkJoin, map } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-teacher-dashboard',
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.scss']
})
export class TeacherDashboardComponent implements OnInit {
  currentSemester: any = null;
  allSemesters: any[] = [];
  selectedSemesterId: number | null = null;
  showButton: boolean = false;
  showStudentList: boolean = false;
  showAttendanceList: boolean = false;
  showAbsenceModalFlag: boolean = false;
  alreadyHasGradeWarningShown: boolean = false;
  showStudentGradesModal: boolean = false;
  showAddGradeModal: boolean = false;
  selectedGrade: { id: number, value: number, date: string } = { id: 0, value: 0, date: '' };
  showEditGradeModal: boolean = false;
  classDisciplines: any[] = [];
  selectedClassDiscipline: any;
  students: any[] = [];
  subjects: any[] = [];
  newGrade: any = {};
  newAttendance: any = {};
  attendanceDate: string = new Date().toISOString().split('T')[0];
  selectedStudent: any = {
    data: null,
    grades: [],
    absences: []
  };
  selectedStudentId: number | null = null;
  selectedSubjectId: number | null = null;
  selectedMonth: number | null = null;
  absences: any[] = [];
  constructor(
    private teacherService: TeacherService,
    private authService: AuthService,
    private api: ApiService,
    private toastr: ToasterService
  ) { }

  ngOnInit(): void {
    this.api.getCurrentSemester().subscribe(
      (semester: any) => {
        this.currentSemester = semester;
      },
      error => {
        console.error('Error fetching current semester:', error);
      }
    );

    this.api.getAllSemesters().subscribe(
      (semesters: any[]) => {
        this.allSemesters = semesters;
      },
      error => {
        console.error('Error fetching all semesters:', error);
      }
    );

    const currentUser = this.authService.getCurrentUser();
    const teacherCNP = currentUser.CNP;
    this.teacherService.getTeacherId(teacherCNP).subscribe(
      (teacherId: number) => {
        this.teacherService.getClassDisciplines(teacherId).subscribe(
          (classDisciplines: any[]) => {
            this.classDisciplines = classDisciplines;
          },
          error => {
            console.error('Error fetching class disciplines:', error);
          }
        );
      },
      error => {
        console.error('Error fetching current teacher ID:', error);
      }
    );
  }

  onClassDisciplineSelect(classDiscipline: any) {
    this.selectedClassDiscipline = classDiscipline;
    this.fetchStudents(classDiscipline.class.classId);
    this.showButton = true;
    this.showStudentList = false;
    this.showAttendanceList = false;
  }

  fetchStudents(classId: number) {
    this.api.getClassStudents(classId).subscribe(
      (students: any[]) => {
        this.students = students;
      },
      error => {
        console.error('Error fetching class students:', error);
      }
    );
  }

  showAbsenceModal() {
    this.showAbsenceModalFlag = true;
    this.selectedStudentId = null;
    this.selectedMonth = null;
    this.absences = [];
  }

  onStudentSelect() {
    this.selectedSemesterId = null;
    this.absences = [];
  }

  onSemesterSelect() {
    if (this.selectedStudentId && this.selectedClassDiscipline?.subject.id && this.selectedSemesterId) {
      this.teacherService.getAbsencesByStudentAndSemester(this.selectedStudentId, this.selectedClassDiscipline.subject.id, this.selectedSemesterId).subscribe(
        (absences: any[]) => {
          this.absences = absences;
        },
        (error: HttpErrorResponse) => {
          if (error.status === 404) {
            this.absences = [];
          } else {
            console.error('Error fetching absences:', error);
          }
        }
      );
    }
  }

  closeAbsenceModal() {
    this.showAbsenceModalFlag = false;
    this.selectedStudentId = null;
    this.selectedSemesterId = null;
    this.absences = [];
  }

  openAddGradeModal(student: any) {
    this.selectedStudent.data = student;
    this.showAddGradeModal = true;
    this.showStudentGradesModal = false;
    this.newGrade.date = new Date().toISOString().split('T')[0];
  }

  openModal(student: any) {
    this.openAddGradeModal(student);
  }

  closeModal() {
    this.selectedStudent.data = null;
    this.showAddGradeModal = false;
    this.showStudentGradesModal = false;
  }

  openEditGradeModal(grade: any) {
    this.selectedGrade = { id: grade.id, value: grade.value, date: grade.date };
    const localDate = new Date(this.selectedGrade.date);
    localDate.setMinutes(localDate.getMinutes() - localDate.getTimezoneOffset());
    this.selectedGrade.date = localDate.toISOString().split('T')[0];
    this.showEditGradeModal = true;
  }

  closeEditGradeModal() {
    this.showEditGradeModal = false;
  }

  showStudentGrades(student: any) {
    this.selectedStudent.data = student;
    this.fetchStudentGradesBySemester(student.id, this.selectedClassDiscipline.subject.id);
  }

  getSemesterNameById(semesterId: number): string {
    const semester = this.allSemesters.find(s => s.id === semesterId);
    return semester ? semester.name : 'Unknown Semester';
  }

  fetchStudentGradesBySemester(studentId: number, subjectId: number) {
    const gradeRequests = this.allSemesters.map(semester =>
      this.teacherService.getGradesByStudentAndSubjectAndSemester(studentId, subjectId, semester.id).pipe(
        map(grades => ({ semesterId: semester.id, grades }))
      )
    );

    forkJoin(gradeRequests).subscribe(
      (gradesBySemester: any[]) => {
        this.selectedStudent.gradesBySemester = gradesBySemester;
        this.selectedStudent.averageGrade = this.calculateAverageGrade(gradesBySemester.flatMap((gs: any) => gs.grades));
        this.showStudentGradesModal = true;
      },
      error => {
        console.error('Error fetching student grades by semester:', error);
      }
    );
  }


  calculateAverageGrade(grades: any[]): number {
    const total = grades.reduce((sum, grade) => sum + grade.value, 0);
    return grades.length > 0 ? total / grades.length : 0;
  }
  hasGradesForSemester(semesterId: number): boolean {
    if (this.selectedStudent && this.selectedStudent.grades) {
      return this.selectedStudent.grades.some((grade: { semesterId: number }) => grade.semesterId === semesterId);
    }
    return false;
  }

  closeStudentGradesModal() {
    this.showStudentGradesModal = false;
  }

  closeAddGradeModal() {
    this.showAddGradeModal = false;
  }

  addGradeOrAttendance() {
    if (this.newGrade.value !== undefined) {
      this.addGrade();
    } else if (this.newAttendance.isPresent !== undefined && this.newAttendance.date !== undefined) {
      const existingAttendance = this.selectedStudent.attendance
        .find((attendance: any) => attendance.date === this.newAttendance.date);
      if (existingAttendance) {
        this.toastr.error('Attendance already added for this date');
        return;
      }
      this.addAttendance();
    }

    if (!this.alreadyHasGradeWarningShown) {
      this.closeModal();
    }
    this.alreadyHasGradeWarningShown = false;
  }

  resetFormFields() {
    this.newGrade.value = undefined;
    this.newGrade.date = undefined;
    this.newAttendance.isPresent = undefined;
    this.newAttendance.date = undefined;
  }

  addGrade() {
    const grade = {
      studentId: this.selectedStudent.data.id,
      subjectId: this.selectedClassDiscipline.subject.id,
      classId: this.selectedClassDiscipline.class.classId,
      value: this.newGrade.value,
      date: this.newGrade.date
    };

    const activeSemester = this.allSemesters.find(semester => !semester.isClosed);

    if (!activeSemester) {
      console.log(activeSemester)
      this.toastr.warning('No active semester found or the semester is closed.');
      return;
    }

    const gradeDate = new Date(grade.date);
    const semesterStartDate = new Date(activeSemester.startDate);
    const semesterEndDate = new Date(activeSemester.endDate);

    if (gradeDate < semesterStartDate || gradeDate > semesterEndDate) {
      this.toastr.warning('Grade date must be within the current open semester.');
      return;
    }

    if (grade.value < 1 || grade.value > 10) {
      this.toastr.warning('Grade must be between 1 and 10');
      return;
    }

    const existingGrade = this.selectedStudent.grades.find((g: any) => g.date === this.newGrade.date);
    if (existingGrade) {
      this.toastr.warning('Student already has a grade for this date');
      this.alreadyHasGradeWarningShown = true;
      return;
    }

    this.teacherService.addGrade(grade).subscribe(
      () => {
        this.toastr.succes('Grade added successfully');
        this.resetFormFields();
      },
      (error) => {
        this.toastr.warning('Failed to add grade: ' + error.message);
      }
    );
  }
  hasGradesBySemester(): boolean {
    return this.selectedStudent.gradesBySemester && this.selectedStudent.gradesBySemester.every((s: any) => s.grades.length === 0);
  }
  editGrade() {
    const updatedGrade = {
      studentId: this.selectedStudent.data.id,
      subjectId: this.selectedClassDiscipline.subject.id,
      classId: this.selectedClassDiscipline.class.classId,
      value: this.selectedGrade.value,
      date: new Date(this.selectedGrade.date).toISOString()
    };

    if (updatedGrade.value < 1 || updatedGrade.value > 10) {
      this.toastr.warning('Grade must be between 1 and 10');
      return;
    }

    this.teacherService.updateGrade(updatedGrade).subscribe(
      () => {
        this.toastr.succes('Grade updated successfully');
        this.fetchStudentGradesBySemester(this.selectedStudent.data.id, this.selectedClassDiscipline.subject.id);
        this.showStudentGradesModal = true;
        this.closeEditGradeModal();
      },
      (error) => {
        this.toastr.error('Error updating grade: ' + error.message);
      }
    );
  }

  deleteGrade() {
    if (!this.selectedGrade.id) {
      this.toastr.warning('Please enter a grade ID');
      return;
    }
    const gradeId = this.selectedGrade.id;
    this.teacherService.deleteGrade(gradeId).pipe(
      finalize(() => {
      })
    ).subscribe(
      () => {
        this.toastr.succes('Grade deleted successfully');
        this.fetchStudentGradesBySemester(this.selectedStudent.data.id, this.selectedClassDiscipline.subject.id);
        this.showEditGradeModal = false;
      },
      (error) => {
        if (error.status === 404) {
          this.toastr.error('Grade not found');
        } else {
          this.toastr.error('Error deleting grade');
          console.error('Error deleting grade', error);
        }
      }
    );
  }


  addAttendance() {
    const attendance = {
      studentId: this.selectedStudent.data.id,
      subjectId: this.selectedClassDiscipline.subject.id,
      classId: this.selectedClassDiscipline.class.classId,
      isPresent: this.newAttendance.isPresent,
      date: this.newAttendance.date
    };
    this.teacherService.addAttendance(attendance).subscribe(
      () => {
        this.toastr.succes('Attendance added successfully');
        this.resetFormFields();
      },
      (error) => {
        this.toastr.error('Failed to add attendance: ' + error.message);
      }
    );
  }

  showStudentsWithGradesModal() {
    if (!this.selectedClassDiscipline) {
      console.error('No class discipline selected');
      return;
    }

    const classId = this.selectedClassDiscipline.class.classId;
    const subjectId = this.selectedClassDiscipline.subject.id;

    this.api.getAttendanceByClassAndDate(classId, subjectId, this.attendanceDate).subscribe(
      (studentsWithGrades: any[]) => {
        this.students = studentsWithGrades.map(student => ({
          ...student,
          grades: (student.grades as any[]).filter(grade => grade.subjectId === subjectId)
        }));
        this.showStudentList = true;
      },
      error => {
        console.error('Error fetching students with grades:', error);
      }
    );
  }

  closeStudentsModal() {
    this.showStudentList = false;
  }

  showAttendanceModal() {
    this.fetchAttendanceRecords();
    this.showAttendanceList = true;
  }

  closeAttendanceModal() {
    this.showAttendanceList = false;
  }

  toggleAttendance(student: any) {
    student.isPresent = !student.isPresent;
  }

  saveAttendance() {
    const attendanceRequests = this.students.map(student => {
      const attendance = {
        studentId: student.id,
        subjectId: this.selectedClassDiscipline.subject.id,
        classId: this.selectedClassDiscipline.class.classId,
        isPresent: student.isPresent,
        date: this.attendanceDate
      };
      return this.teacherService.addAttendance(attendance);
    });

    forkJoin(attendanceRequests).subscribe(
      () => {
        this.toastr.succes('Attendance saved successfully');
        this.closeAttendanceModal();
      },
      error => {
        this.toastr.warning('Attendance not saved. Please try again.');
        console.error('Error saving attendance:', error);
      }
    );
  }

  fetchStudentGrades(studentId: number, subjectId: number) {
    this.api.getGradesByStudentAndSubject(studentId, subjectId).subscribe(
      (grades: any[]) => {
        this.selectedStudent.grades = grades;
      },
      error => {
        console.error('Error fetching student grades:', error);
      }
    );
  }

  fetchAttendanceRecords() {
    this.students.forEach(student => {
      student.isPresent = false;
    });

    this.api.getAttendanceByClassAndDate(this.selectedClassDiscipline.class.classId, this.selectedClassDiscipline.subject.id, this.attendanceDate).subscribe(
      (attendanceRecords: any[]) => {
        if (attendanceRecords && attendanceRecords.length > 0) {
          this.students.forEach(student => {
            const record = attendanceRecords.find(record => record.studentId === student.id);
            student.isPresent = record ? record.isPresent : false;
          });
        } else {
          console.log('No attendance records found for the given class, subject, and date.');
        }
      },
      (error: HttpErrorResponse) => {
        if (error.status === 404) {
          console.log('No attendance records found for the given class, subject, and date.');
        } else {
          console.error('Error fetching attendance records:', error);
        }
      }
    );
  }
  logout() {
    this.authService.signOut();
  }
}
