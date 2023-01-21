<template>
  <b-container class="book-user-list">

    <b-alert class="mt-2" v-model="showDismissibleAlert" variant="success" dismissible>
      Password succesfully changed!</b-alert>
    <b-alert class="mt-2" v-model="showDismissibleAlertError" variant="danger" dismissible>
     {{errorMessage}}</b-alert>
    <b-form @submit="verifyPassword" v-if="show" class="user-form">
      <b-row class="form-row">
        <b-col cols="5"><p class="form-label required" >Old password:</p> </b-col>
        <b-col cols="7">
          <b-form-input
              v-model="form.oldPassword"
              placeholder="Enter old password"
              type="password"
              required>
          </b-form-input>
        </b-col>
      </b-row>
      <b-row class="form-row">
        <b-col cols="5"><p class="form-label required" >New password:</p> </b-col>
        <b-col cols="7">
          <b-form-input
              v-model="form.password"
              placeholder="Enter new password"
              type="password"
              required>
          </b-form-input>
        </b-col>
      </b-row>
      <b-row class="form-row">
        <b-col cols="5"><p class="form-label required" >Confirm password:</p> </b-col>
        <b-col cols="7">
          <b-form-input
              v-model="form.confirmPassword"
              placeholder="Confirm new password"
              type="password"
              required>
          </b-form-input>
        </b-col>
      </b-row>
      <b-button type="submit" class="submit-button" >Submit</b-button>
    </b-form>

  </b-container>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";

export default {
  name: 'ChangePassword',
  data() {
    return {
      show: true,
      showDismissibleAlert: false,
      showDismissibleAlertError: false,
      errorMessage: "Some error occured"
    };
  },
  props: {
    user: {},
    form: {},
  },
  methods: {
    onSubmit() {
      ApiConnect.put('readers/', this.form).then(response => {
        this.showDismissibleAlert=true;
        // window.location.reload();
      }).catch(error => {
        this.errorMessage = "Some error occured."
        this.showDismissibleAlertError=true;
        this.showDismissibleAlert=false;
      })
    },
    verifyPassword(){
      if (this.form.password !== this.form.confirmPassword) {
        this.errorMessage = "Password must match!"
        this.showDismissibleAlertError=true;
        this.showDismissibleAlert=false;
        return;
      }
      if (this.form.password === this.form.oldPassword){
        this.errorMessage = "New password cannot be the same like old one."
        this.showDismissibleAlertError=true;
        this.showDismissibleAlert=false;
        return;
      }
      ApiConnect.put('/readers/update-password', this.form).then(response=>{
        this.showDismissibleAlert=true;
        this.showDismissibleAlertError=false;
      }).catch(error => {
        this.errorMessage = "Old password doesnt match.";
        this.showDismissibleAlertError=true;
      })
    }
  }

}
</script>