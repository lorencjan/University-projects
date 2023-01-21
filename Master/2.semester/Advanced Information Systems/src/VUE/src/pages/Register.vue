<template>
  <div id="register">
    <NavbarFinal></NavbarFinal>
    <b-container>
      <div id="register-box" class="mt-4 border border-primary p-5">
        <b-row>
          <b-col>
            <h1 class="text-center">Register</h1>
          </b-col>
        </b-row>
        <b-form @submit.prevent="submit" @keyup.enter.prevent="submit" id="form-1" novalidate>
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
                <b-form-invalid-feedback>
                  Name field can not be empty.
                </b-form-invalid-feedback>
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
                <b-form-invalid-feedback>
                  Surname field can not be empty.
                </b-form-invalid-feedback>
              </b-form-group>
            </b-col>
          </b-row>
          <b-row>
<!--            <b-col>-->
<!--              <b-form-group-->
<!--                  id="input-group-3"-->
<!--                  label="Full name:"-->
<!--                  label-for="name"-->
<!--              >-->
<!--                <b-form-input-->
<!--                    id="name"-->
<!--                    v-model="form.fullname"-->
<!--                    type="text"-->
<!--                    placeholder="Enter your full name"-->
<!--                    autocomplete="name"-->
<!--                    required-->
<!--                ></b-form-input>-->
<!--              </b-form-group>-->
<!--            </b-col>-->
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
          <hr>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-5"
                  label="City:"
                  label-for="city"
              >
                <b-form-input
                    id="city"
                    v-model="form.city"
                    type="text"
                    placeholder="Enter your city"
                    autocomplete="address-level2"
                ></b-form-input>
              </b-form-group>
            </b-col>
            <b-col>
              <b-form-group
                  id="input-group-6"
                  label="Street:"
                  label-for="street"
              >
                <b-form-input
                    id="street"
                    v-model="form.street"
                    type="text"
                    placeholder="Enter your street"
                ></b-form-input>
              </b-form-group>
            </b-col>
          </b-row>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-7"
                  label="House number:"
                  label-for="houseNumber"
              >
                <b-form-input
                    id="houseNumber"
                    v-model="form.houseNumber"
                    type="number"
                    placeholder="Enter your house number"
                ></b-form-input>
              </b-form-group>
            </b-col>
            <b-col>
              <b-form-group
                  id="input-group-8"
                  label="Post code:"
                  label-for="postcode"
              >
                <b-form-input
                    id="postcode"
                    v-model="form.postcode"
                    type="number"
                    placeholder="Enter your post code"
                    autocomplete="postal-code"
                ></b-form-input>
              </b-form-group>
            </b-col>
          </b-row>
          <hr>
          <b-row>
            <b-col>
              <b-form-group
                  id="input-group-9"
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
                  id="input-group-9"
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
                  Passwords are not same.
                </b-form-invalid-feedback>
              </b-form-group>
            </b-col>
          </b-row>
          <b-row>
            <b-col class="text-center">
              <b-button @click="submit" variant="primary">Register</b-button>
            </b-col>
          </b-row>
          <p v-if="this.showError" style="font-color:red"> {{errMessage}}</p>
          <b-row class="mt-3">
            <b-col class="text-center">
              Already have account? <router-link to="/login/">Login</router-link>
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

export default {
  name: "Register",
  components: {
    NavbarFinal
  },
  data() {
    return {
      form: {
        id: 0,
        name: "",
        surname: "",
        email: "",
        city: "",
        street: "",
        houseNumber: "",
        postcode: "",
        role: "READER",
        password: "",
        fullname: ""
      },
      showError: false,
      errMessage: ""
    };
  },
  methods: {
    reset_form_state(){
      this.$refs['fname'].state = null;
      this.$refs['lname'].state = null;
      this.$refs['email'].state = null;
      this.$refs['password'].state = null;
      this.$refs['confirm-password'].state = null;
    },
    submit(){
      this.reset_form_state();
      let form_required_error = false;
      if (!this.form.name) {
        this.$refs['fname'].state = false;
        this.$refs['fname'].value = "";
        this.form.password = "";
        this.form.confirmPassword = "";
        form_required_error = true;
      }

      if (! this.form.surname){
        this.$refs['lname'].state = false;
        this.$refs['lname'].value = "";
        this.form.password = "";
        this.form.confirmPassword = "";
        form_required_error = true;
      }
      if (!this.form.email){
        this.$refs['email'].state = false;
        this.$refs['email'].valueOf = "";
        this.errMessage = "Email address can not be empty.";
        this.form.password = "";
        this.form.confirmPassword = "";
        form_required_error = true;
      } else{
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
      if (form_required_error) return;

      if (this.form.password != this.form.confirmPassword){
        this.$refs['password'].state = false;
        this.$refs['password'].value = "";
        this.$refs['confirm-password'].state = false;
        this.$refs['confirm-password'].value = "";
        return;
      }


      ApiConnect.post('/readers', JSON.stringify(this.form), ApiConnect.headers).then((response) => {
        if (response.status == 200)
        {
          let user = {}
          user.email = this.form.email;
          user.password = this.form.password;
          ApiConnect.post('/readers/authenticate', user, ApiConnect.headers).then((response) =>{
            if (response)
            {
              localStorage.setItem('id', JSON.stringify(response.data.id));
              localStorage.setItem('role', JSON.stringify(response.data.role));
              this.$router.push('/');
            }
            this.showError = false
          }).catch(error => {
            console.log(error);
          })

        }}
      ).catch(error => {
        console.log(error);
        this.$refs['email'].state=false;
        this.errMessage = "Email already registered.";
      })
    }
  }
}
</script>
<style scoped>
#register-box{
  text-align: left;
  color: black;
  margin: auto;
  border-radius: 15px;
}
</style>

