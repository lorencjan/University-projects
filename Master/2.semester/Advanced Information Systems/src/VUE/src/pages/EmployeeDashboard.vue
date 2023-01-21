<template>
  <div>
    <NavbarFinal></NavbarFinal>
    <b-container class="bv-example-row">
      <b-row align-h="center">
        <b-col cols="3">
          <router-link to="/edit_titles" class="router-link-dashboard">
            <b-card
              text-variant="white"
              class="pb-4"
              >
            <b-card-title
              class="text-center text-light"
            >
              Edit Titles
            </b-card-title>
            <b-icon icon="book"
                    class="h1"
                    style="width: 120px; height: 120px">
            </b-icon>
          </b-card>
        </router-link>
        </b-col>
        <b-col cols="3">
          <router-link to="/edit_authors" class="router-link-dashboard">
            <b-card
              text-variant="white"
              class="pb-4"
            >
              <b-card-title
                  class="text-center text-light"
              >
                Edit Authors
              </b-card-title>
              <b-icon icon="person-badge"
                      class="h1"
                      style="width: 120px; height: 120px">
              </b-icon>
            </b-card>
          </router-link>
        </b-col>
        <b-col cols="3">
          <router-link to="/edit_borrowings" class="router-link-dashboard">
            <b-card
                text-variant="white"
                class="pb-4"
            >
              <b-card-title
                  class="text-center text-light"
              >
                Edit Borrowings
              </b-card-title>
              <b-icon icon="bookmark"
                      class="h1"
                      style="width: 120px; height: 120px">
              </b-icon>
            </b-card>
          </router-link>
        </b-col>
      </b-row>
      <b-row align-h="center" class="mt-4">
        <b-col cols="3">
          <router-link to="/edit_fines" class="router-link-dashboard">
            <b-card
                text-variant="white"
                class="pb-4">
              <b-card-title
                  class="text-center text-light"
              >
                Edit Fines
              </b-card-title>
              <b-icon icon="cash-stack"
                      class="h1"
                      style="width: 120px; height: 120px">
              </b-icon>
            </b-card>
          </router-link>
        </b-col>
        <b-col cols="3">
          <router-link to="/edit_reservations" class="router-link-dashboard">
            <b-card
                text-variant="white"
                class="pb-4"
            >
              <b-card-title
                  class="text-center text-light"
              >
                Edit Reservations
              </b-card-title>
              <b-icon icon="journal-bookmark-fill"
                      class="h1"
                      style="width: 120px; height: 120px">
              </b-icon>
            </b-card>
          </router-link>
        </b-col>
        <b-col cols="3">
          <router-link to="/edit_readers" class="router-link-dashboard">
            <b-card
                text-variant="white"
                class="pb-4"
            >
              <b-card-title
                  class="text-center text-light"
              >
                Edit Readers
              </b-card-title>
              <b-icon icon="person"
                      class="h1"
                      style="width: 120px; height: 120px">
              </b-icon>
            </b-card>
          </router-link>
        </b-col>
      </b-row>
      <b-row align-h="center" class="mt-4" v-show="isAdmin">
        <b-col cols="4">
        </b-col>
        <b-col cols="4">
          <router-link to="/edit_employees/" class="router-link-dashboard">
          <b-card text-variant="white" class="pb-4">
            <b-card-title class="text-center text-light">
              Edit Employees
            </b-card-title>
            <b-icon icon="person-plus" class="h1" style="width: 120px; height: 120px">
            </b-icon>
          </b-card>
          </router-link>
        </b-col>
        <b-col cols="4">
        </b-col>
      </b-row>
    </b-container>

  </div>
</template>

<script>
import { BootstrapVue, BootstrapVueIcons } from 'bootstrap-vue';
import Vue from "vue";
import { faDisplay, faUserTie} from "@fortawesome/free-solid-svg-icons";
import { library } from '@fortawesome/fontawesome-svg-core';
import NavbarFinal from "@/components/main_page/NavbarFinal";
library.add(faDisplay, faUserTie);

Vue.use(BootstrapVue)
Vue.use(BootstrapVueIcons)

export default {
  name: "EmployeeDashboard",
  components: {
    NavbarFinal
  },
  computed: {
    isAdmin : function (){return (localStorage.getItem('role') == "\"ADMIN\"")}
  }
}
</script>

<style scoped>
a{
  text-decoration: none;
}

.card{
  border-radius: 0.25rem;
  background: #24433e;
  box-shadow: 0 6px 10px rgba(0,0,0,.08), 0 0 6px rgba(0,0,0,.05);
  transition: .3s transform cubic-bezier(.155,1.105,.295,1.12),.3s box-shadow,.3s -webkit-transform cubic-bezier(.155,1.105,.295,1.12);
  cursor: pointer;
}

.card:hover{
  transform: scale(1.05);
  box-shadow: 0 10px 20px rgba(0,0,0,.12), 0 4px 8px rgba(0,0,0,.06);
}
</style>