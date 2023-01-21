<template>
  <div id="mynavbar">
    <b-container>

    <b-navbar toggleable="lg" variant="info">
      <b-navbar-brand left href="#">
        <router-link to="/">
        <b-img @click="deleteSearch" class="navbar-logo" src="@/assets/library_logo.png" alt="Responsive image" ></b-img>
        </router-link>
      </b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>

        <!-- Right aligned nav items -->
        <b-navbar-nav class="ml-auto">
          <b-nav-item :to="{path: '/books/'}">Catalog</b-nav-item>
          <b-nav-item-dropdown id="my-dropdown" text="Genre" right>
            <BookGenre v-for="load in this.genre_names" :genre=load.name :id=load.id :key="load.id" />
          </b-nav-item-dropdown>
          <b-nav-item-dropdown id="my-dropdown" text="Field" right>
            <MagazineField v-for="load in this.field_names" :field=load.name :id=load.id :key="load.id" />
          </b-nav-item-dropdown>
         <div v-if="isLoggedIn" class="d-flex">
            <b-nav-item @click="logout">
              <font-awesome-icon icon="fa-solid fa-arrow-right-from-bracket"/>
            </b-nav-item>
            <b-nav-item @click="home">
              <font-awesome-icon icon="fa-solid fa-house"/>
            </b-nav-item>
            <b-nav-item v-show="isReader" @click="userProfile">
              <font-awesome-icon icon="fa-solid fa-user-circle"/>
            </b-nav-item>
          </div>
          <div v-else class="d-flex">
            <b-nav-item :to="{path: '/login/'}">Login</b-nav-item>
            <b-nav-item :to="{path: '/register/'}">Register</b-nav-item>
            <b-nav-item :to="{path: '/login_employee'}">Employees</b-nav-item>
          </div>
        </b-navbar-nav>

      </b-collapse>

    </b-navbar>
    </b-container>
    <SearchBar v-on="$listeners"></SearchBar>
  </div>

</template>

<script>

import SearchBar from "@/components/main_page/SearchBar";
import BookGenre from "@/components/main_page/BookGenre";
import MagazineField from "@/components/main_page/MagazineField";
import ApiConnect from "@/services/ApiConnect";
import { library } from '@fortawesome/fontawesome-svg-core';
import { faDisplay, faHouse, faArrowRightFromBracket, faUserCircle} from "@fortawesome/free-solid-svg-icons";
library.add(faDisplay, faHouse, faArrowRightFromBracket, faUserCircle)

export default {
  name: 'NavBar',
  components: {
    SearchBar,
    BookGenre,
    MagazineField
  },
  computed: {
    isLoggedIn : function (){ return (localStorage.getItem('id') != null)},
    isReader : function (){return (localStorage.getItem('role') == "\"READER\"")}
  },
  methods : {
    logout (){
      let who = localStorage.getItem('role');
      localStorage.removeItem('id');
      localStorage.removeItem('role');
      if (who == "\"READER\"")
      {
        this.$router.push('/login')
      }
      else
      {
        this.$router.push('/login_employee')
      }
    },
    getGenres(){
      ApiConnect.get('genres/').then((response) =>
            this.genre_names = response.data
      )},
    getFields(){
      ApiConnect.get('fields/').then((response) =>
          this.field_names = response.data
      )},
    userProfile() {
      this.$router.push('/readers/' + localStorage.getItem('id'));
    },
    home() {
      if(this.isReader)
      {
        this.$router.push('/');
      }
      else
      {
        this.$router.push('/employee_dashboard/');
      }
    },
    deleteSearch(){
      this.search_input = '';
      this.$emit('deleteSearch', '');
    }
  },
  data(){
    return {
      genre_names : [],
      field_names : [],
      search_input: ''
    }
  },
  created() {
    this.getGenres(),
        this.getFields()
  }
}
</script>