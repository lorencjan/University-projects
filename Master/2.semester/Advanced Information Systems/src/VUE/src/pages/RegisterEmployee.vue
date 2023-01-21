<template>
    <div id="register_employee">
      <NavbarFinal></NavbarFinal>
        <b-container>
      <div id="register-box" class="mt-4 border border-primary p-5">
        <b-row>
          <b-col v-if="notRegistered">
            <h1 class="text-center">Register</h1>
          </b-col>
          <b-col v-else>
            <h1 class="text-center">Edit</h1>
          </b-col>
        </b-row>
        <b-form @submit.prevent="submit" id="form-1" novalidate>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-1"
                  label="Name:"
                  label-class="required"
                  label-for="fname"
              >
                <b-form-input
                    id="fname"
                    ref="fname"
                    v-model="form.name"
                    type="text"
                    placeholder="Enter your name"
                    autocomplete="given-name"
                    required
                ></b-form-input>
              </b-form-group>
            </b-col>
            <b-col>
              <b-form-group
                  id="input-group-2"
                  label="Surname:"
                  label-class="required"
                  label-for="lname"
              >
                <b-form-input
                    id="lname"
                    ref="lname"
                    v-model="form.surname"
                    type="text"
                    placeholder="Enter your surname"
                    autocomplete="family-name"
                    required
                ></b-form-input>
              </b-form-group>
            </b-col>
          </b-row>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-3"
                  label="Full name:"
                  label-for="name"
              >
                <b-form-input
                    id="name"
                    v-model="form.fullname"
                    type="text"
                    placeholder="Enter your full name"
                    autocomplete="name"
                ></b-form-input>
              </b-form-group>
            </b-col>
            <b-col>
              <b-form-group
                  id="input-group-4"
                  label="Email:"
                  label-class="required"
                  label-for="email"
              >
                <b-form-input
                    ref="email"
                    id="email"
                    v-model="form.email"
                    type="email"
                    placeholder="Enter email"
                    autocomplete="email"
                    required
                ></b-form-input>
                <b-form-invalid-feedback>
                  {{ errMessage }}
                </b-form-invalid-feedback>
              </b-form-group>
            </b-col>
          </b-row>
          <b-row>
              <b-col>
                  <b-form-group
                  id="input-group-5"
                  label="Role:"
                  label-class="required"
                  label-for="role">
<!--                      <b-dropdown id="role_dropdown" :text="selectedRole">-->
<!--                          <b-dropdown-item-button @click="selectedRole = 'Employee'">Employee</b-dropdown-item-button>-->
<!--                          <b-dropdown-item-button @click="selectedRole = 'Admin'">Admin</b-dropdown-item-button>-->
<!--                      </b-dropdown>-->
                    <multiselect
                        label="text"
                        track-by="text"
                        placeholder="Type to search"
                        v-model="selectedRole"
                        :options="options">
                    </multiselect>
                  </b-form-group>
              </b-col>
              <b-col>
              </b-col>
          </b-row>
          <hr>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-6"
                  label="City:"
                  label-for="city"
              >
                <b-form-input
                    id="city"
                    v-model="form.city"
                    type="text"
                    placeholder="Enter your city"
                    autocomplete="address-level2"
                    required
                ></b-form-input>
              </b-form-group>
            </b-col>
            <b-col>
              <b-form-group
                  id="input-group-7"
                  label="Street:"
                  label-for="street"
              >
                <b-form-input
                    id="street"
                    v-model="form.street"
                    type="text"
                    placeholder="Enter your street"
                    required
                ></b-form-input>
              </b-form-group>
            </b-col>
          </b-row>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-8"
                  label="House number:"
                  label-for="houseNumber"
              >
                <b-form-input
                    id="houseNumber"
                    v-model="form.houseNumber"
                    type="number"
                    placeholder="Enter your house number"
                    required
                ></b-form-input>
              </b-form-group>
            </b-col>
            <b-col>
              <b-form-group
                  id="input-group-9"
                  label="Post code:"
                  label-for="postcode"
              >
                <b-form-input
                    id="postcode"
                    v-model="form.postcode"
                    type="number"
                    placeholder="Enter your post code"
                    autocomplete="postal-code"
                    required
                ></b-form-input>
              </b-form-group>
            </b-col>
          </b-row>
          <hr>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-10"
                  label="Password:"
                  label-class="required"
                  label-for="password"
              >
                <b-form-input
                    id="password"
                    ref="password"
                    v-model="form.password"
                    type="password"
                    placeholder="Enter your password"
                    autocomplete="new-password"
                    required
                ></b-form-input>
              </b-form-group>
            </b-col>
            <b-col>
              <b-form-group
                  id="input-group-10"
                  label="Confirm password:"
                  label-class="required"
                  label-for="confirm-password"
              >
                <b-form-input
                    ref="confirm-password"
                    id="confirm-password"
                    v-model="form.confirmPassword"
                    type="password"
                    placeholder="Confirm your password"
                    autocomplete="new-password"
                    required
                ></b-form-input>
                <b-form-invalid-feedback>
                  Passwords are not the same.
                </b-form-invalid-feedback>
              </b-form-group>
            </b-col>
            <b-row v-if="showError">
              <p style="color: red">{{errMessage}} </p>
            </b-row>
          </b-row>
          <b-row>
            <b-col class="text-center" v-if="notRegistered">
              <b-button @click="register" variant="primary">Register</b-button>
            </b-col>
            <b-col class="text-center" v-else>
              <b-button @click="submit" variant="primary">Submit</b-button>
            </b-col>
          </b-row>
        </b-form>
      </div>
    </b-container>
    </div>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";
import NavbarFinal from "@/components/main_page/NavbarFinal";
import Multiselect from "vue-multiselect";

export default {
  name: "RegisterEmployee",
  components: {
    NavbarFinal,
    Multiselect
  },
  computed: {
    notRegistered : function (){return this.$route.params.id == 0}
  },
  data() {
    return {
      employee: {},
      form: {
        id: 0,
        name: "",
        surname: "",
        email: "",
        city: "",
        street: "",
        houseNumber: "",
        postcode: "",
        role: "",
        password: "",
        fullname: ""
      },
      showError: false,
      errMessage: "",
      selectedRole: { value: 'EMPLOYEE', text: 'Employee' },
      options: [
        { value: 'EMPLOYEE', text: 'Employee' },
        { value: 'ADMIN', text: 'Admin' },
      ]
    };
  },
  methods: {
    check_form(){
      let form_required_error = false;
      if (!this.form.name) {
        console.log('a');
        this.$refs['fname'].state = false;
        this.$refs['fname'].value = "";
        this.form.password = "";
        this.form.confirmPassword = "";
        form_required_error = true;
      }

      if (! this.form.surname){
        console.log('b');
        this.$refs['lname'].state = false;
        this.$refs['lname'].value = "";
        this.form.password = "";
        this.form.confirmPassword = "";
        form_required_error = true;
      }
      if (!this.form.email){
        console.log('a');
        this.$refs['email'].state = false;
        this.$refs['email'].valueOf = "";
        this.errMessage = "Email address can not be empty.";
        this.form.password = "";
        this.form.confirmPassword = "";
        form_required_error = true;
      }
      else {
        var mailformat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
        if( ! this.form.email.match(mailformat))
        {
          this.$refs['email'].state = false;
          this.$refs['email'].valueOf = "";
          this.errMessage = "Please enter valid email address.";
          this.form.password = "";
          this.form.confirmPassword = "";
          form_required_error = true;
        }
      }
      if (! this.selectedRole){
        console.log('a');
        this.showError = true;
        this.errMessage = "Role can not be empty";
        form_required_error = true;
      }
      if (form_required_error) return form_required_error;
      if (this.form.password.length < 1){
        console.log('a');
        this.$refs['password'].state = false;
        this.$refs['password'].value = "";
        this.$refs['confirm-password'].state = false;
        this.$refs['confirm-password'].value = "";
        return true;
      }
      if (this.form.password != this.form.confirmPassword){
        console.log('a');
        this.$refs['password'].state = false;
        this.$refs['password'].value = "";
        this.$refs['confirm-password'].state = false;
        this.$refs['confirm-password'].value = "";
        return true;
      }
    },
    reset_form_state(){
      this.$refs['fname'].state = null;
      this.$refs['lname'].state = null;
      this.$refs['email'].state = null;
      this.$refs['password'].state = null;
      this.$refs['confirm-password'].state = null;
    },
    register(){
      this.reset_form_state();
      this.form.role = this.selectedRole.value;
      if (this.check_form()) return;
        ApiConnect.post('/employees', JSON.stringify(this.form), ApiConnect.headers).then((response) => 
          {
            console.log(this.form);
            this.$router.push('/login_employee');
          }
        ).catch(error => {
        if (error.response) {
          if (error.response.status == 400)
          {
            this.showError = true;
            this.errMessage = "This email address is already in use. Please, select another email."
            this.$refs['email'].state = false;
            this.$refs['email'].value = "";
          }
          else
          {
            this.errMessage = "This email address is already in use. Please, select another email."  
          }
        }
      })
    },
    makeToast(text) {
      this.$bvToast.toast(text, {
        title: 'Library',
        variant: 'success',
        autoHideDelay: 5000,
      })
    },
    submit() {
      this.reset_form_state();
      this.form.role = this.selectedRole.value;
      if (this.check_form()) return;
      ApiConnect.put('/employees', JSON.stringify(this.form), ApiConnect.headers).then((response) =>
      {
        this.reset_form_state();
        this.makeToast('Employee '+this.employee.name+' ' + this.employee.surname + 'has been updated successfully.')
        if (response.data)
        {
          if (response.data.status != 200)
          {
            this.showError = true
            this.errMessage = "Update of informations did not save corectly. Try again.";
          }
        }
      }
      ).catch((error) =>
      {
        this.errMessage = "Update of informations did not save. Try again.";
      })
    },
    getEmployee(){
      if (this.$route.params.id != 0){
        ApiConnect.get('/employees/' + this.$route.params.id).then((response) =>
            {
              console.log(response);
              this.employee = response.data;
              this.form.id = this.employee.id;
              this.form.name = this.employee.name;
              this.form.surname = this.employee.surname;
              this.form.email = this.employee.email;
              this.form.city = this.employee.city;
              this.form.street = this.employee.street;
              this.form.houseNumber = this.employee.houseNumber;
              this.form.postcode = this.employee.postcode;
              this.form.role = this.employee.role;
              this.selectedRole = this.employee.role == "EMPLOYEE" ? { value: 'EMPLOYEE', text: 'Employee' } : { value: 'ADMIN', text: 'Admin' };
              this.form.password = "";
              this.form.fullname = this.employee.fullname;
            }
        ).catch((error) =>
        console.log(error))
      }
    },
    Employee(){
        this.form.role = "EMPLOYEE";
    },
    Admin(){
        this.form.role = "ADMIN";
    }
  },
  created(){
    this.getEmployee();
  }
}
</script>
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
<style scoped>
#register-box{
  text-align: left;
  color: black;
  margin: auto;
  border-radius: 15px;
}
</style>
