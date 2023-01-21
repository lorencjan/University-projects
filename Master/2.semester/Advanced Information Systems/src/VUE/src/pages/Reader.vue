<template>
  <div id="app">
    <NavbarFinal></NavbarFinal>
    <b-container>

      <div v-if="user.hardCopyBorrowings != undefined">
        <div v-if="user.hardCopyBorrowings.length != 0">

          <b-alert v-if="fineAmount > 0" class="mt-2" show variant="danger">
            You have unpaid fines in amount {{fineAmount}}CZK!</b-alert>
        </div>
      </div>

      <b-row>
        <b-col cols="6">
          <UserForm  :user="user"
                     :form="form"
          ></UserForm>
        </b-col>
        <b-col cols="6">
          <ChangePassword :user="user"
                          :form="formPassword">
          </ChangePassword>
        </b-col>
      </b-row>

      <div v-if="user.reservations != undefined">
        <div v-if="user.reservations.length != 0">
          <BookList type="Reservations"
                    :data="user.reservations"
                    :user="user"
                    :borrowing="false"
          ></BookList>
        </div>
      </div>

      <div v-if="user.hardCopyBorrowings != undefined">
        <div v-if="user.hardCopyBorrowings.length != 0">
          <BookList type="Borrowings"
                    :data="user.hardCopyBorrowings"
                    :user="user"
                    :borrowing="true"
          ></BookList>
        </div>
      </div>
      <div v-if="user.electronicCopyBorrowings != undefined ">
        <div v-if="user.electronicCopyBorrowings.length != 0">
          <BookList type="Electronic Borrowings"
                    :data="user.electronicCopyBorrowings"
                    :user="user"
                    :electronic="true"
                    :borrowing="true"
          ></BookList>
        </div>
      </div>

      <!--      <BookList type="Borrowings"  name="Harry Potter" date="2022-05-31" state="OK"></BookList>-->
    </b-container>
  </div>
</template>

<script>
import BookList from "@/components/user_page/BookList";
import UserForm from "@/components/user_page/UserForm";
import ApiConnect from "@/services/ApiConnect";
import ChangePassword from "@/components/user_page/ChangePassword";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: 'UserPage',
  components: {
    BookList,
    UserForm,
    ChangePassword,
    NavbarFinal
  },
  computed: {
    id() {
      return this.$route.params.id
    },
    fineAmount: function (){
      let fines = 0;
      for (let borrowing of this.user.hardCopyBorrowings) {
        for (let fine of borrowing.fines){
          if (fine.state !== 'PAID'){
              fines +=fine.amount;
          }
        }
      }
      return fines;
    }
  },
  data(){
    return{
      user: {},
      form: {
        id: '',
        name: '',
        surname: '',
        email: '',
        street: '',
        houseNumber: '',
        city: '',
        postcode: '',
      },
      formPassword: {
        userId: '',
        password: '',
        confirmPassword: '',
        oldPassword: ''
      }

    }
  },
  methods: {
    getReader(){
      let id = this.$route.params.id;
      if (typeof(this.$route.params.id) == 'undefined'){
        id = ''
      }
      ApiConnect.get('readers/' + id).then((response)=> {
        this.user = response.data;
        this.getFormData();
        this.formPassword.userId = this.$route.params.id;
      })
    },
    getFormData(){
      this.form.id = this.$route.params.id;
      this.form.name=this.user.name;
      this.form.surname=this.user.surname;
      this.form.email=this.user.email;
      this.form.street=this.user.street;
      this.form.houseNumber=this.user.houseNumber;
      this.form.city=this.user.city;
      this.form.postcode=this.user.postcode;
    },
  },
  created() {
    this.getReader();
  },
  
}
</script>

<style>
  @import "../assets/styles/main.css";
  @import "../assets/styles/user.css";
</style>