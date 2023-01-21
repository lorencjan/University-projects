<template>
  <div id="login_employee">
    <NavbarFinal></NavbarFinal>
    <b-container>
      <div id="login-box" class="mt-4 border border-primary p-5">
        <b-row><h3 class="text-center" style="font-color: black">We see. You are our colleague.</h3></b-row>
        <b-row>
          <b-col>
            <h1 class="text-center">Log in</h1>
          </b-col>
        </b-row>
        <b-form @submit.prevent="submit" @keyup.enter.prevent="submit" novalidate>
          <b-form-group
              id="email-label"
              label="Email address:"
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
          </b-form-group>

          <b-form-group
              id="password-label"
              label="Your password:"
              label-class="required"
              label-for="password"
          >
            <b-form-input
                ref="password"
                id="password"
                type="password"
                v-model="form.password"
                placeholder="Enter password"
                autocomplete="current-password"
                required
            ></b-form-input>
            <b-form-invalid-feedback>
              {{ errMessage }}
            </b-form-invalid-feedback>
          </b-form-group>
          <b-row>
            <b-col class="text-center">
              <b-button @click="submit" variant="primary">Login</b-button>
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
  name: "Login_Employee",
  components: {
    NavbarFinal
  },
  data() {
    return {
    form: {
      email: "",
      password: "",
    },
    errMessage: ""
  };
  },
  methods: {
    submit(){
      if (localStorage.getItem('id') != null)
      {
        this.errMessage = "Another user is already logged in.";
        return;
      }
      if (!this.form.email){
        this.errMessage = "Please fill in email before trying to login in."
        this.$refs['password'].state = false;
        this.$refs['password'].value = "";
        this.form.password = "";
        this.$refs['email'].state = false;
        this.$refs['email'].value = "";
        return;
      }
      if (!this.form.password){
        this.errMessage = "Please fill in password before trying to login in."
        this.$refs['password'].state = false;
        this.$refs['password'].value = "";

        return;
      }
      const data = {email: this.form.email, password: this.form.password};
      ApiConnect.post('/employees/authenticate', JSON.stringify(data), ApiConnect.headers).then((response) =>
        {
          if(response.status == 200)
          {
            localStorage.setItem('id', JSON.stringify(response.data.id));
            localStorage.setItem('role', JSON.stringify(response.data.role));
            this.$router.push('/employee_dashboard');
          }
        }
      ).catch(error => {
        if (error.response) {
          if (error.response.status == 404)
          {
            this.errMessage = "Employee with this credentials was not found."
          }
          else
          {
            this.errMessage = "Error ocured on server side. Please, try again later."  
          }
        }
        else
        {
          this.errMessage = "Error ocured on server side. Please, try again later."
        }
        this.$refs['password'].state = false;
        this.$refs['password'].value = "";
        this.$refs['email'].state = false;
        this.$refs['email'].value = "";
      })
      }
  }
}
</script>

<style scoped>
#login-box{
  text-align: left;
  color: black;
  max-width: 500px;
  margin: auto;
  border-radius: 15px;
  box-shadow: 0 10px 20px rgba(0,0,0,.12), 0 4px 8px rgba(0,0,0,.06);
}
</style>
