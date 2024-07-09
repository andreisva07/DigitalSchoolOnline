import { Component, EventEmitter } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent {
  public users: any = [];
  public fullName: string = '';
  public role!: string;
  public teachers: any[] = [];
  public students: any[] = [];
  public subjects: any[] = [];
  public availableClasses: any[] = [];
  public availableSubjects: any[] = [];
  public selectedTeacher: any;
  public selectedSubject: any;
  public teacherId!: any;
  public studentId!: any;
  public subjectId!: any;
  public classId!: any;
  public semesterId!: any;
  public selectedSemesterId: number | null = null;
  public selectedSubjectId: number | null = null;
  public selectedTeacherId: number | null = null;
  public selectedClassId: number | null = null;
  public selectedTeacherForDeletion: any;
  public showTeachersList: boolean = false;
  public showStudentsList: boolean = false;
  public showSubjectsList: boolean = false;
  public showClassesList: boolean = false;
  public showAddStudentFormFlag: boolean = false;
  public showAddTeacherFormFlag: boolean = false;
  public showAddSubjectFormFlag: boolean = false;
  public showAddClassFormFlag: boolean = false;
  public showAddTeacherSubjectModalFlag: boolean = false;
  public showDeleteTeacherSubjectModalFlag = false;
  public showDeleteClassModalFlag = false;
  public showDeleteTeacherModalFlag = false;
  public showDeleteStudentModalFlag = false;
  public showDeleteSubjectModalFlag = false;
  public showAddTeacherClassModalFlag: boolean = false;
  public showDeleteTeacherClassModalFlag: boolean = false;
  public showDeleteSemesterModalFlag: boolean = false;
  public selectedClassForStudents: number | null = null;
  public studentsInSelectedClass: any[] = [];
  public selectedStudentForTransfer: number | null = null;
  public newClassForTransfer: number | null = null;
  public showTransferStudentModalFlag: boolean = false;
  public showViewClassStudentsModalFlag: boolean = false;
  public showViewTeacherSubjectsModalFlag: boolean = false;
  public selectedTeacherForSubjects: number | null = null;
  public teacherClassDisciplineLinks: any[] = [];
  public selectedClassDisciplineLink: any | null = null;
  public availableClassesForTransfer: any[] = [];
  public showEditStudentModalFlag: boolean = false;
  public selectedStudent: any = {};
  public subjectsOfSelectedTeacher: any[] = [];
  public selectedUserType: string = 'Student';
  public studentsWithDetails: any[] = [];
  public teachersWithDetails: any[] = [];
  selectedRole: string | null = null;
  public newTeacherClass: any = { teacherId: null, classId: null };
  public closeModal = new EventEmitter<void>();
  public genders: string[] = ['Male', 'Female', 'NonBinary'];
  public showSemesterModalFlag: boolean = false;
  public showAddSemesterModalFlag: boolean = false;
  public closedSemesters: any[] = [];
  public semesters: any[] = [];
  public selectedClosedSemesterId: number | null = null;
  public newSemester: any = {
    name: '',
    startDate: '',
    endDate: ''
  };
  public newStudent: any = {
    firstName: '',
    lastName: '',
    gender: '',
    Cnp: '',
    classId: null
  };
  public newTeacherSubject: any = { teacherId: null, subjectId: null };

  public newSubject: any = {
    name: ''
  };
  public newTeacher: any = {
    firstName: '',
    lastName: '',
    Cnp: ''
  };
  public newClass: any = {
    series: '',
    number: ''

  };

  public selectedOption: string = '';

  constructor(
    private auth: AuthService,
    private api: ApiService,
    private userStore: UserStoreService,
    private toastr: ToastrService,
  ) { }

  ngOnInit() {
    this.fetchSemesters();
    this.fetchClosedSemesters();
    this.fetchTeachers();
    this.fetchStudents();
    this.fetchSubjects();
    this.fetchAvailableClasses();

    this.api.getUsers().subscribe((res) => {
      this.users = res;
    });

    this.userStore.getFullNameFromStore().subscribe((val) => {
      const fullNameFromToken = this.auth.getFullNameFromToken();
      this.fullName = val || fullNameFromToken;
    });

    this.userStore.getRoleFromStore().subscribe((val) => {
      const roleFromToken = this.auth.getRoleFromTOken();
      this.role = val || roleFromToken;
    });
  }

  openAddTeacherModal() {
    this.showAddTeacherFormFlag = true;
  }

  closeAddTeacherModal() {
    this.showAddTeacherFormFlag = false;
  }

  openDeleteTeacherModal() {
    this.showDeleteTeacherModalFlag = true;
  }

  closeDeleteTeacherModal() {
    this.showDeleteTeacherModalFlag = false;
  }

  openAddStudentModal() {
    this.showAddStudentFormFlag = true;
  }

  closeAddStudentModal() {
    this.showAddStudentFormFlag = false;
  }

  openDeleteStudentModal() {
    this.showDeleteStudentModalFlag = true;
  }

  closeDeleteStudentModal() {
    this.showDeleteStudentModalFlag = false;
  }

  openAddDisciplineModal() {
    this.showAddSubjectFormFlag = true;
  }

  closeAddDisciplineModal() {
    this.showAddSubjectFormFlag = false;
  }

  openDeleteDisciplineModal() {
    this.showDeleteSubjectModalFlag = true;
  }

  closeDeleteDisciplineModal() {
    this.showDeleteSubjectModalFlag = false;
  }

  openAddClassModal() {
    this.showAddClassFormFlag = true;
  }

  closeAddClassModal() {
    this.showAddClassFormFlag = false;
  }

  openDeleteClassModal() {
    this.showDeleteClassModalFlag = true;
  }

  closeDeleteClassModal() {
    this.showDeleteClassModalFlag = false;
  }

  openAddTeacherSubjectModal() {
    this.resetTeacherSubjectForm();
    this.newTeacherSubject.subjectId = null;
    this.showAddTeacherSubjectModalFlag = true;
  }

  closeAddTeacherSubjectModal() {
    this.resetTeacherSubjectForm();
    this.showAddTeacherSubjectModalFlag = false;
  }

  openDeleteTeacherSubjectModal() {
    this.resetTeacherSubjectForm();
    this.showDeleteTeacherSubjectModalFlag = true;
  }

  closeDeleteTeacherSubjectModal() {
    this.resetTeacherSubjectForm();
    this.showDeleteTeacherSubjectModalFlag = false;
  }

  openAddTeacherClassModal() {
    this.resetTeacherClassForm();
    this.newTeacherClass.classId = null;
    this.newTeacherClass.subjectId = null;
    this.showAddTeacherClassModalFlag = true;
    this.fetchAvailableClasses();
  }

  closeAddTeacherClassModal() {
    this.resetTeacherClassForm();
    this.showAddTeacherClassModalFlag = false;
    this.fetchAvailableClasses();
  }

  openDeleteTeacherClassModal() {
    this.resetTeacherClassForm();
    this.showDeleteTeacherClassModalFlag = true;
  }

  closeDeleteTeacherClassModal() {
    this.resetTeacherClassForm();
    this.showDeleteTeacherClassModalFlag = false;
  }
  openSemesterModal() {
    this.showSemesterModalFlag = true;
    this.fetchSemesters();
  }

  closeSemesterModal() {
    this.showSemesterModalFlag = false;
  }
  openAddSemesterModal() {
    this.showAddSemesterModalFlag = true;
  }

  closeAddSemesterModal() {
    this.showAddSemesterModalFlag = false;
  }
  openDeleteSemesterModal() {
    this.showDeleteSemesterModalFlag = true;
  }

  fetchAvailableClasses() {
    this.api.getAvailableClasses().subscribe((res) => {
      this.availableClasses = res;
    });
  }
  closeDeleteSemesterModal(): void {
    this.showDeleteSemesterModalFlag = false;
  }
  openClassSelectionModal() {
    this.showTransferStudentModalFlag = true;
  }

  closeTransferStudentModal() {
    this.showTransferStudentModalFlag = false;
    this.selectedClassForStudents = null;
    this.studentsInSelectedClass = [];
    this.selectedStudentForTransfer = null;
    this.newClassForTransfer = null;
  }
  openViewClassStudentsModal() {
    this.showViewClassStudentsModalFlag = true;
  }

  closeViewClassStudentsModal() {
    this.showViewClassStudentsModalFlag = false;
  }

  openViewTeacherSubjectsModal() {
    this.showViewTeacherSubjectsModalFlag = true;
  }

  closeViewTeacherSubjectsModal() {
    this.showViewTeacherSubjectsModalFlag = false;
  }

  openEditStudentModal(student: any) {
    this.selectedStudent = { ...student };
    this.showEditStudentModalFlag = true;
    console.log('Selected student for edit:', this.selectedStudent);
  }


  closeEditStudentModal() {
    this.showEditStudentModalFlag = false;
  }

  fetchStudentsByClassWithDetails(classId: number | null) {
    if (classId !== null) {
      this.api.getStudentsByClassWithDetails(classId).subscribe(
        (students) => {
          this.studentsWithDetails = students;
        },
        (error) => {
          this.toastr.error('Error fetching students. Please try again.');
          console.error('Error fetching students:', error);
        }
      );
    }
  }

  fetchAllTeachersWithDetails() {
    this.api.getAllTeachersWithDetails().subscribe(
      (teachers) => {
        this.teachersWithDetails = teachers;
      },
      (error) => {
        this.toastr.error('Error fetching teachers. Please try again.');
        console.error('Error fetching teachers:', error);
      }
    );
  }

  fetchStudentsByClass(classId: number | null) {
    if (classId !== null) {
      this.api.getStudentsByClassWithDetails(classId).subscribe(
        (students: any[]) => {
          this.studentsInSelectedClass = students.map(student => ({
            id: student.id,
            firstName: student.firstName,
            lastName: student.lastName,
            username: student.username,
            password: student.password
          }));
          this.filterAvailableClassesForTransfer();
        },
        (error) => {
          this.toastr.error('Error fetching students. Please try again.');
          console.error('Error fetching students:', error);
        }
      );
    }
  }

  filterAvailableClassesForTransfer() {
    if (this.selectedClassForStudents !== null) {
      const currentClass = this.availableClasses.find(cls => cls.classId === this.selectedClassForStudents);
      if (currentClass) {
        this.availableClassesForTransfer = this.availableClasses.filter(cls => cls.number === currentClass.number && cls.classId !== currentClass.classId);
      }
    }
  }


  fetchSubjectsByTeacher(teacherId: number | null) {
    if (teacherId !== null) {
      this.api.getSubjectsByTeacher(teacherId).subscribe(
        (subjects) => {
          this.subjectsOfSelectedTeacher = subjects;
        },
        (error) => {
          this.toastr.error('Error fetching subjects. Please try again.');
          console.error('Error fetching subjects:', error);
        }
      );
    }
  }
  fetchSemesters() {
    this.api.getAllSemesters().subscribe(
      (res) => {
        this.semesters = res;
      },
      (error) => {
        this.toastr.error('Error fetching semesters. Please try again.');
        console.error('Error fetching semesters:', error);
      }
    );
  }
  fetchClosedSemesters() {
    this.api.getClosedSemesters().subscribe(
      (data) => {
        this.closedSemesters = data;
      },
      (error) => {
        this.toastr.error('Error fetching closed semesters. Please try again.');
      }
    );
  }
  fetchTeachers() {
    this.api.getTeachers().subscribe((res) => {
      this.teachers = res;
    });
  }

  fetchStudents() {
    this.api.getStudents().subscribe((res) => {
      this.students = res;
    });
  }

  fetchSubjects() {
    this.api.getSubjects().subscribe((res) => {
      this.subjects = res;
    });
  }
  fetchavailableSemesters() {
    this.api.getAllSemesters().subscribe((res) => {
      this.semesters = res;
    })
  }
  selectOption(option: string) {
    this.selectedOption = option;
    if (option === 'teachers') {
      this.fetchTeachers();
    } else if (option === 'students') {
      this.fetchStudents();
    }
  }

  onUserTypeChange() {
    this.studentsWithDetails = [];
    this.teachersWithDetails = [];
    if (this.selectedUserType === 'student' && this.selectedClassForStudents) {
      this.fetchStudentsByClassWithDetails(this.selectedClassForStudents);
    } else if (this.selectedUserType === 'teacher') {
      this.fetchAllTeachersWithDetails();
    }
  }

  onClassChange() {
    if (this.selectedUserType === 'student' && this.selectedClassForStudents) {
      this.fetchStudentsByClassWithDetails(this.selectedClassForStudents);
    }
  }

  editStudent() {
    console.log('Updating student:', this.selectedStudent);
    this.api.updateStudent(this.selectedStudent.id, this.selectedStudent).subscribe(
      response => {
        console.log('Response from server:', response);
        this.toastr.success('Student details updated successfully!');
        this.closeEditStudentModal();
        this.fetchStudentsByClass(this.selectedClassForStudents);
      },
      error => {
        console.error('Error updating student details:', error);
        this.toastr.error('Error updating student details. Please try again.');
      }
    );
  }


  transferStudent() {
    if (this.selectedStudentForTransfer !== null && this.newClassForTransfer !== null) {
      console.log('Selected Student ID:', this.selectedStudentForTransfer);
      console.log('New Class ID:', this.newClassForTransfer);

      this.api.transferStudent(this.selectedStudentForTransfer, this.newClassForTransfer).subscribe(
        (response) => {
          this.toastr.success(response.message || 'Student transferred successfully!');
          this.closeTransferStudentModal();
        },
        (error) => {
          console.error('Selected Student ID:', this.selectedStudentForTransfer);
          console.error('New Class ID:', this.newClassForTransfer);
          this.toastr.error('Error transferring student. Please try again.');
          console.error('Error transferring student:', error);
        }
      );
    } else {
      this.toastr.warning('Please select both a student and a new class.');
    }
  }


  addTeacher() {
    const { firstName, lastName, Cnp } = this.newTeacher;
    if (firstName && lastName) {
      this.api.addProfessor(firstName, lastName, Cnp).subscribe(
        (response) => {
          this.toastr.success('Teacher added successfully!');
          this.fetchTeachers();
          this.newTeacher = { firstName: '', lastName: '' };
          this.closeAddTeacherModal();
        },
        (error) => {
          this.toastr.error('Error adding teacher. Please try again.');
          console.error('Error adding teacher:', error);
        }
      );
    } else {
      this.toastr.warning('Please enter both first name and last name.');
    }
  }

  deleteTeacher() {
    if (!this.teacherId) {
      this.toastr.warning('Please enter a teacher ID');
      return;
    }

    const teacherId = this.teacherId;

    this.api.deleteTeacher(teacherId).pipe(
      finalize(() => {
        this.teacherId = null;
        this.closeDeleteTeacherModal();
      })
    ).subscribe(
      () => {
        this.toastr.success('Teacher deleted successfully');
        this.fetchTeachers();
      },
      (error) => {
        if (error.status === 404) {
          this.toastr.error('Teacher not found');
        } else {
          this.toastr.warning('Teacher deleted successfully');
          this.fetchTeachers();
          console.error('Error deleting teacher', error);
        }
      }
    );
  }

  addStudent() {
    const { firstName, lastName, gender, classId, Cnp } = this.newStudent;
    const genderValue = this.genders.indexOf(gender);
    if (firstName && lastName && gender && classId && Cnp !== null && !isNaN(classId)) {
      this.api.addStudent(firstName, lastName, genderValue, Cnp, +classId).subscribe(
        (response: any) => {
          this.toastr.success('Student added successfully!');
          this.fetchStudents();
          this.newStudent = { firstName: '', lastName: '', gender: '', Cnp: '', classId: null };
          this.closeAddStudentModal();
        },
        (error: any) => {
          this.toastr.error('Error adding student. Please try again.');
        }
      );
    } else {
      this.toastr.warning('Please enter first name, last name, gender, and select a class.');
    }
  }

  deleteStudent() {
    if (!this.studentId) {
      this.toastr.warning('Please enter a student ID');
      return;
    }

    const studentId = this.studentId;

    this.api.deleteStudent(studentId).pipe(
      finalize(() => {
        this.studentId = null;
        this.closeDeleteStudentModal();
      })
    ).subscribe(
      () => {
        this.toastr.warning('Student deleted successfully');
        this.fetchStudents();
      },
      (error) => {
        if (error.status === 404) {
          this.toastr.error('Student not found');
        } else {
          this.toastr.error('Error deleting student');
          console.error('Error deleting student', error);
        }
      }
    );
  }

  addSubject() {
    const { name } = this.newSubject;
    if (name) {
      this.api.addSubject({ name }).subscribe(
        (response) => {
          this.toastr.success('Discipline added successfully!');
          this.fetchSubjects();
          this.newSubject = { name: '' };
          this.closeAddDisciplineModal();
        },
        (error) => {
          this.toastr.error('Error adding discipline. Please try again.');
          console.error('Error adding discipline:', error);
        }
      );
    } else {
      this.toastr.warning('Please enter the name of the discipline.');
    }
  }

  deleteSubject() {
    if (!this.subjectId) {
      this.toastr.warning('Please enter a discipline ID');
      return;
    }

    const subjectId = this.subjectId;

    this.api.deleteSubject(subjectId).pipe(
      finalize(() => {
        console.log(subjectId);
        this.subjectId = null;
        this.closeDeleteDisciplineModal();
      })
    ).subscribe(
      () => {
        this.toastr.success('Discipline deleted successfully');
        this.fetchSubjects();
      },
      (error) => {
        if (error.status === 404) {
          this.toastr.error('Discipline not found');
        } else {
          this.toastr.error('Error deleting discipline');
          console.error('Error deleting discipline', error);
        }
      }
    );
  }

  addClass() {
    const { series, number } = this.newClass;
    if (series && number) {
      this.api.addClass(series, number).subscribe(
        () => {
          this.toastr.success('Class added successfully!');
          this.fetchAvailableClasses();
          this.newClass = { series: '', number: '' };
          this.closeAddClassModal();
        },
        (error) => {
          this.toastr.error('Error adding class. Please try again.');
          console.error('Error adding class:', error);
        }
      );
    } else {
      this.toastr.warning('Please enter both series and number for the class.');
    }
  }

  onClassSelect() {
    this.fetchStudentsByClass(this.selectedClassForStudents);
    this.filterAvailableClassesForTransfer();
  }

  deleteClass() {
    if (!this.classId) {
      this.toastr.warning('Please enter a class ID');
      return;
    }

    const classId = this.classId;

    this.api.deleteClass(classId).pipe(
      finalize(() => {
        this.classId = null;
        this.closeDeleteClassModal();
      })
    ).subscribe(
      () => {
        this.toastr.success('Class deleted successfully');
        this.fetchAvailableClasses();
      },
      (error) => {
        if (error.status === 404) {
          this.toastr.error('Class not found');
        } else {
          this.toastr.error('Error deleting class');
          console.error('Error deleting class', error);
        }
      }

    );
  }

  deleteSemester() {
    if (!this.selectedSemesterId) {
      this.toastr.warning('Please enter a class ID');
      return;
    }
    const semesterId = this.selectedSemesterId;

    this.api.deleteSemester(semesterId).pipe(
      finalize(() => {
        this.selectedSemesterId = null;
        this.closeDeleteSemesterModal();
      })
    ).subscribe(
      () => {
        this.toastr.success('Semester deleted successfully');
        this.fetchavailableSemesters();
      },
      (error) => {
        if (error.status === 404) {
          this.toastr.error('Class not found');
        } else {
          this.toastr.error('Error deleting class');
          console.error('Error deleting class', error);
        }
      }
    );
  }

  getClassId(event: Event) {
    const target = event.target as HTMLSelectElement;
    const selectedOption = target.value;
    if (selectedOption) {
      const [series, number] = selectedOption.split(' - ');
      this.api.getClassId(series, number).subscribe(
        (classId: number) => {
          console.log(`Class ID for series ${series} and number ${number} is ${classId}`);
          this.newStudent.classId = classId;
          this.newClassForTransfer = classId;
        },
        (error) => {
          this.toastr.error('Error fetching class ID. Please try again.');
        }
      );
    }
  }


  getTeacherId(event: Event) {
    const target = event.target as HTMLSelectElement;
    const selectedOption = target.options[target.selectedIndex].value;
    const selectedTeacher = this.teachers.find(teacher => teacher.id === +selectedOption);
    if (selectedTeacher) {
      const { firstName, lastName } = selectedTeacher;
      this.api.getTeacherId(firstName, lastName).subscribe(
        (teacherId: number) => {
          this.newTeacherSubject.teacherId = teacherId;
          console.log('Teacher ID:', teacherId);
          this.fetchAvailableSubjectsForTeacher(teacherId);
        },
        (error) => {
          console.error('Error fetching teacher ID:', error);
        }
      );
    } else {
      console.error('Selected teacher not found in the list of teachers.');
    }
  }

  fetchAvailableSubjectsForTeacher(teacherId: number) {
    this.api.getAvailableSubjectsForTeacher(teacherId).subscribe(
      (availableSubjects: any[]) => {
        this.availableSubjects = availableSubjects;
        console.log(availableSubjects);
      },
      (error) => {
        console.error('Error fetching available subjects for teacher:', error);
      }
    );
  }

  fetchLinkedSubjectsForTeacher(event: Event) {
    const target = event.target as HTMLSelectElement;
    const teacherId = target.value;

    if (teacherId) {
      this.api.getLinkedSubjectsForTeacher(+teacherId).subscribe(
        (linkedSubjects: any[]) => {
          this.availableSubjects = linkedSubjects;
        },
        (error) => {
          console.error('Error fetching linked subjects for teacher:', error);
        }
      );
    }
  }

  createTeacherSubjectLink() {
    if (!this.newTeacherSubject.teacherId || !this.newTeacherSubject.subjectId) {
      this.toastr.warning('Please provide both teacher and subject details.');
      return;
    }

    const teacherId = this.newTeacherSubject.teacherId;
    const subjectId = this.newTeacherSubject.subjectId;

    const teacherSubjectData = {
      teacherId,
      subjectId
    };

    this.api.addTeacherSubject(teacherSubjectData).subscribe(
      response => {
        this.toastr.success('Teacher-subject link created successfully!');
        this.resetTeacherSubjectForm();
        this.closeAddTeacherSubjectModal();
      },
      error => {
        this.toastr.error('Error creating teacher-subject link. Please try again.');
      }
    );
  }

  deleteTeacherSubjectLink() {
    if (!this.newTeacherSubject.teacherId || !this.newTeacherSubject.subjectId) {
      this.toastr.warning('Please provide both teacher and subject details.');
      return;
    }

    const teacherId = this.newTeacherSubject.teacherId;
    const subjectId = this.newTeacherSubject.subjectId;

    this.api.deleteTeacherSubjectLink(teacherId, subjectId).subscribe(
      response => {
        this.toastr.success('Teacher-subject link deleted successfully!');
        this.resetTeacherSubjectForm();
        this.closeDeleteTeacherSubjectModal();
        this.fetchLinkedSubjectsForTeacher({ target: { value: teacherId } } as any);
      },
      error => {
        this.toastr.error('Error deleting teacher-subject link. Please try again.');
        console.error('Error deleting teacher-subject link:', error);
      }
    );
  }
  fetchTeacherClassDisciplineLinks(event: Event) {
    const target = event.target as HTMLSelectElement;
    const teacherId = target.value;

    if (teacherId) {
      this.api.getTeacherClassDisciplineLinks(+teacherId).subscribe(
        (links: any[]) => {
          this.teacherClassDisciplineLinks = links;
        },
        (error) => {
          console.error('Error fetching teacher-class-discipline links:', error);
        }
      );
    }
  }

  deleteTeacherClassDisciplineLink() {
    if (this.selectedClassDisciplineLink === null) {
      this.toastr.warning('Please select a class-discipline link to delete.');
      return;
    }

    const { teacherId, classId, subjectId } = this.selectedClassDisciplineLink;

    this.api.deleteTeacherClassDisciplineLink({ teacherId, classId, subjectId }).subscribe(
      (response) => {
        this.toastr.success('Teacher-class-discipline link deleted successfully!');
        this.resetTeacherClassForm();
        this.fetchTeacherClassDisciplineLinks({ target: { value: this.newTeacherClass.teacherId } } as any);
      },
      (error) => {
        this.toastr.error('Error deleting teacher-class-discipline link. Please try again.');
        console.error('Error deleting teacher-class-discipline link:', error);
      }
    );
  }

  createTeacherClassLink() {
    if (!this.newTeacherClass.teacherId || !this.newTeacherClass.classId || !this.newTeacherClass.subjectId) {
      this.toastr.warning('Please provide teacher, class, and subject details.');
      return;
    }

    this.api.checkTeacherClassSubjectExists(this.newTeacherClass.classId, this.newTeacherClass.subjectId).subscribe(
      (exists: boolean) => {
        if (exists) {
          this.toastr.error('This subject is already assigned to another teacher for the selected class.');
        } else {
          const teacherClassData = {
            teacherId: this.newTeacherClass.teacherId,
            classId: this.newTeacherClass.classId,
            subjectId: this.newTeacherClass.subjectId
          };

          this.api.addTeacherClassLink(teacherClassData).subscribe(
            response => {
              this.toastr.success('Teacher-Class-Subject link created successfully!');
              this.resetTeacherClassForm();
              this.closeAddTeacherClassModal();
              this.fetchAvailableClasses();
            },
            error => {
              this.toastr.error('Error creating teacher-class-subject link. Please try again.');
              console.error('Error creating teacher-class-subject link:', error);
            }
          );
        }
      },
      error => {
        this.toastr.error('Error checking subject assignment. Please try again.');
        console.error('Error checking subject assignment:', error);
      }
    );
  }


  getTeacherIdForClass(event: Event) {
    const target = event.target as HTMLSelectElement;
    const selectedOption = target.options[target.selectedIndex].value;
    const selectedTeacher = this.teachers.find(teacher => teacher.id === +selectedOption);
    this.newTeacherClass.classId = null;
    this.newTeacherClass.subjectId = null;
    this.availableSubjects = [];
    if (selectedTeacher) {
      const { firstName, lastName } = selectedTeacher;
      this.api.getTeacherId(firstName, lastName).subscribe(
        (teacherId: number) => {
          this.newTeacherClass.teacherId = teacherId;
          console.log('Teacher ID:', teacherId);
          this.fetchAvailableClassesForTeacher(teacherId);
        },
        (error) => {
          console.error('Error fetching teacher ID:', error);
        }
      );
    } else {
      console.error('Selected teacher not found in the list of teachers.');
    }
  }

  fetchAvailableClassesForTeacher(teacherId: number) {
    this.api.getAvailableClassesForTeacher(teacherId).subscribe(
      (availableClasses: any[]) => {
        this.availableClasses = availableClasses;
      },
      (error) => {
        console.error('Error fetching available classes for teacher:', error);
      }
    );
  }


  fetchAvailableSubjectsForTeacherClass(event: Event) {
    const target = event.target as HTMLSelectElement;
    const classId = target.value;
    this.newTeacherClass.subjectId = null;
    if (classId) {
      this.api.getAvailableSubjectsForTeacherClass(this.newTeacherClass.teacherId, +classId).subscribe(
        (availableSubjects: any[]) => {
          this.availableSubjects = availableSubjects;
        },
        (error) => {
          console.error('Error fetching available subjects for teacher and class:', error);
        }
      );
    }
  }
  fetchLinkedClassesForTeacher(event: Event) {
    const target = event.target as HTMLSelectElement;
    const teacherId = target.value;

    if (teacherId) {
      this.api.getLinkedClassesForTeacher(+teacherId).subscribe(
        (linkedClasses: any[]) => {
          this.availableClasses = linkedClasses;
        },
        (error) => {
          console.error('Error fetching linked classes for teacher:', error);
        }
      );
    }
  }


  resetTeacherClassForm() {
    this.newTeacherClass = {
      teacherId: null,
      classId: null,
      subjectId: null,
    };
    this.teacherClassDisciplineLinks = [];
    this.selectedClassDisciplineLink = null;
  }
  resetTeacherSubjectForm() {
    this.newTeacherSubject = {
      teacherId: null,
      subjectId: null
    };
    this.availableSubjects = [];
  }
  addSemester() {
    this.api.addSemester(this.newSemester).subscribe(
      (response) => {
        this.toastr.success('Semester added successfully!');
        this.fetchSemesters();
        this.newSemester = { name: '', startDate: '', endDate: '' };
        this.closeAddSemesterModal();
      },
      (error) => {
        this.toastr.error('Error adding semester. Please try again.');
        console.error('Error adding semester:', error);
      }
    );
  }

  closeSemester(semesterId: number) {
    this.api.closeSemester(semesterId).subscribe(
      (response) => {
        this.toastr.success('Semester closed successfully!');
        this.fetchSemesters();
        this.fetchClosedSemesters();

      },
      (error) => {
        this.toastr.error('Error closing semester. Please try again.');
        console.error('Error closing semester:', error);
      }
    );
  }

  reactivateSemester() {
    const activeSemester = this.semesters.find(semester => !semester.isClosed);
    if (activeSemester) {
      this.toastr.warning('There is already an active semester. Please close the active semester before reactivating another one.');
      return;
    }
    console.log('Selected closed semester ID:', this.selectedClosedSemesterId);
    if (this.selectedClosedSemesterId) {
      this.api.reactivateSemester(this.selectedClosedSemesterId).subscribe(
        (response) => {
          this.toastr.success('Semester reactivated successfully!');
          this.fetchSemesters();
          this.fetchClosedSemesters();
          this.selectedClosedSemesterId = null;
        },
        (error) => {
          this.toastr.error('Error reactivating semester. Please try again.');
        }
      );
    } else {
      this.toastr.warning('Please select a closed semester to reactivate.');
    }
  }
  fetchDataForRole() {
    if (this.selectedRole === 'student') {
      this.studentsInSelectedClass = [];
      this.selectedClassForStudents = null;
    }
  }

  close() {
    this.closeModal.emit();
  }

  logout() {
    this.auth.signOut();
  }
}
