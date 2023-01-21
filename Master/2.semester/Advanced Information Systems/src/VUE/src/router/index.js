import Vue from 'vue'
import VueRouter from 'vue-router'
import App from '@/pages/App.vue'
import Genre from "@/pages/Genre"
import Login from "@/pages/Login"
import Register from "@/pages/Register"
import User from "@/pages/Reader";
import Books from "@/pages/Books";
import Book from "@/pages/Book";
import Magazines from "@/pages/Magazines";
import Magazine from "@/pages/Magazine";
import AuthorsList from "@/pages/AuthorsList";
import EditAuthor from "@/pages/EditAuthor";
import ReadersList from "@/pages/ReadersList";
import EmployeeList from "@/pages/EmployeeList";
import Authors from "@/pages/Authors";
import GenreAuthors from "@/pages/GenreAuthors";
import Author from "@/pages/Author";
import LoginEmployee from "@/pages/LoginEmployee";
import EmployeeDashboard from "@/pages/EmployeeDashboard";
import TitleList from "@/pages/TitleList";
import BorrowingList from "@/pages/BorrowingList";
import EditBook from "@/pages/EditBook";
import EditMagazine from "@/pages/EditMagazine";
import NotFound from "@/pages/NotFound";
import FineList from "@/pages/FineList";
import ReservationList from "@/pages/ReservationList";
import RegisterEmployee from "@/pages/RegisterEmployee";
import EditGenre from "@/pages/EditGenre";
import EditField from "@/pages/EditField";
import EditHardCopyBorrowing from "@/pages/EditHardCopyBorrowing";
import EditReservation from "@/pages/EditReservation";
import Field from "@/pages/Field";
import FieldAuthors from "@/pages/FieldAuthors";
import CreateReader from "@/pages/CreateReader";
import CreateBorrowing from "@/pages/CreateBorrowing";

Vue.use(VueRouter)

const Routes = [
  {
    path: '/',
    component: App,
    meta: {
      title: 'Library',
    }
  },
  {
    path: '/readers/:id',
    sensitive: true,
    component: User,
    meta: {
      title: 'Reader Edit',
      requiresAuth: true
    },
  },
  {
    path: '/genre/:id?',
    component: Genre,
    meta: {
      title: 'Genre page',
    }
  },
  {
    path: '/field/:id',
    component: Field,
    meta : {
      title: "Field page"
    }
  },
  {
    path: '/login',
    component: Login,
    meta: {
      title: 'Login',
      guest: true
    }
  },
  {
    path: '/login_employee',
    component: LoginEmployee,
    meta: {
      title: 'Login employee',
      guest: true
    }
  },
  {
    path: '/register',
    component: Register,
    meta: {
      title: 'Register',
      guest: true
    }
  },
  {
    path: '/books/',
    component: Books,
    meta: {
      title: 'Books',
    }
  },
  {
    path: '/books/:id',
    component: Book,
    meta: {
      title: 'Book page',
    }
  },
  {
    path: '/edit_books/:id',
    component: EditBook,
    meta: {
      title: 'Book edit',
      employee: true,
    }
  },
  {
    path: '/edit_magazines/:id',
    component: EditMagazine,
    meta: {
      title: 'magazine edit',
      employee: true,
    }
  },
  {
    path: '/authors/',
    component: Authors,
    meta: {
      title: 'Authors',
    }
  },
  {
    path: '/authors/:id',
    component: Author,
    meta: {
      title: 'Author page',
    }
  },
  {
    path: '/genre_authors/:id',
    component: GenreAuthors,
    meta: {
      title: 'Authors by genre',
    }
  },
  {
    path: '/field_authors/:id',
    component: FieldAuthors,
    meta: {
      title: 'Authors by field',
    }
  },
  {
    path: '/magazines/',
    component: Magazines,
    meta: {
      title: 'Magazines',
    }
  },
  {
    path: '/magazines/:id',
    component: Magazine,
    meta: {
      title: 'Magazine page',
    }
  },
  {
    path: '/edit_authors',
    component: AuthorsList,
    meta: {
      title: 'Edit Authors',
      employee: true
    }
  },
  {
    path : '/edit_authors/:id',
    component: EditAuthor,
    meta: {
      title: 'Edit specific author',
      employee: true
    }
  },
  {
    path: '/edit_readers',
    component: ReadersList,
    meta: {
      title: 'Edit readers',
      employee: true,
    }
  },
  {
    path: '/edit_employees',
    component: EmployeeList,
    meta: {
      title: 'Edit employees',
      administrator: true
    }
  },
  {
    path: '/register_employee/:id',
    component: RegisterEmployee,
    meta: {
      title: 'Employee',
      administrator: true
    }
  },
  {
    path: '/employee_dashboard',
    component: EmployeeDashboard,
    meta: {
      title: 'Employee dashboard',
      employee: true
    }
  },
  {
    path: '/edit_titles',
    component: TitleList,
    meta: {
      title: 'Edit titles',
      employee: true
    }
  },
  {
    path: '/edit_borrowings',
    component: BorrowingList,
    meta: {
      title: 'Edit Borrowings',
      employee: true
    }
  },
  {
    path: '/edit_fines',
    component: FineList,
    meta: {
      title: 'Edit Fines',
      employee: true
    }
  },
  {
    path: '/edit_reservations',
    component: ReservationList,
    meta: {
      title: 'Edit Reservations',
      employee: true
    }
  },
  {
    path: '/edit_reservations/:id',
    component: EditReservation,
    meta: {
      title: 'Edit Reservation',
      employee: true
    }
  },
  {
    path: '/edit_genres/:id',
    component: EditGenre,
    meta: {
      title: 'Edit Genre',
      employee: true
    }
  },
  {
    path: '/edit_fields/:id',
    component: EditField,
    meta: {
      title: 'Edit Field',
      employee: true
    }
  },
  {
    path: '/edit_hard-copy-borrowings/0',
    component: CreateBorrowing,
    meta: {
      title: 'Create Borrowing',
      employee: true
    }
  },
  {
    path: '/edit_hard-copy-borrowings/:id',
    component: EditHardCopyBorrowing,
    meta: {
      title: 'Edit Borrowing',
      employee: true
    }
  },
  {
    path: '/createReader',
    component: CreateReader,
    meta: {
      title: 'Create Reader',
      employee: true
    }
  },
  {
    path: '/*',
    component: NotFound,
    meta : {
      title: "404 Not Found"
    }
  }
]

const router = new VueRouter({
  routes: Routes,
});

router.beforeEach((to, from, next) => {
  if(to.matched.some((record)=>record.meta.requiresAuth)) {
    if((localStorage.getItem('id') === to.params.id || (localStorage.getItem('role') === "\"ADMIN\"" || localStorage.getItem('role') === "\"EMPLOYEE\""))) {
      next();
      return;
    }
    next('/');
  }
  else
  {
    next();
  }
});

router.beforeEach((to, from, next) => {
  if (to.matched.some((record)=>record.meta.guest)) {
    if (localStorage.getItem('id')) {
      if (localStorage.getItem('role') == "\"EMPLOYEE\"" || localStorage.getItem('role') == "\"ADMIN\"")
      {
        next("/employee_dashboard")
      }
      else
      {
        next("/");
      }
      return;
    }
    next();
  }
  else {
    next();
  }
});

router.beforeEach((to, from, next) => {
  if(to.matched.some((record)=>record.meta.employee)) {
    let role = localStorage.getItem('role');
    if(localStorage.getItem('id') && role != null && (role === "\"EMPLOYEE\"" || role === "\"ADMIN\"")) {
      next();
      return;
    }
    next("/login_employee");
  }
  else {
    next();
  }
});

router.beforeEach((to, from, next) => {
  if(to.matched.some((record)=>record.meta.administrator)) {
    let role = localStorage.getItem('role');
    if(localStorage.getItem('id') && role != null && (role === "\"ADMIN\"")) {
      next();
      return;
    }
    next("/login_employee");
  }
  else {
    next();
  }
});

const DEFAULT_TITLE = 'PIS library system';
router.afterEach((to, from) => {
  Vue.nextTick(() => {
    document.title = to.meta.title || DEFAULT_TITLE;
  });
});

export default router
