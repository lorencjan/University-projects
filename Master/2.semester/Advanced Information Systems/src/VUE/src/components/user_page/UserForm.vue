<template>
  <b-container class="book-user-list">
    <b-alert class="mt-2" v-model="showDismissibleAlert" variant="success" dismissible>
      Your informations were succesfully updated!</b-alert>
    <b-alert class="mt-2" v-model="showDismissibleAlertError" variant="danger" dismissible>
      There was error while trying to update your information!</b-alert>

        <b-form @submit.prevent="onSubmit"  @keyup.enter.prevent="onSubmit" class="user-form">

          <b-row class="form-row">
            <b-col cols="4"><p class="form-label required">Name:</p> </b-col>
            <b-col cols="8">
              <b-form-input
                v-model="form.name"
                placeholder="Enter first name"
                id="fname"
                required>
              </b-form-input>
            </b-col>
          </b-row>

          <b-row class="form-row">
            <b-col cols="4"><p class="form-label required">Last name:</p> </b-col>
            <b-col cols="8">
              <b-form-input
                v-model="form.surname"
                placeholder="Enter last name"
                id="lname"
                required>
                </b-form-input>
            </b-col>
          </b-row>

          <b-row class="form-row">
            <b-col cols="4"><p class="form-label required">Email:</p></b-col>
            <b-col cols="8">
              <b-form-input
                  id="email"
                  v-model="form.email"
                  type="email"
                  required
                  placeholder="Enter email">
              </b-form-input></b-col>
          </b-row>

          <b-row class="form-row">
            <b-col cols="4"><p class="form-label">Street:</p> </b-col>
            <b-col cols="8">
              <b-form-input
                  id="street"
                  v-model="form.street"
                  placeholder="Enter street name">
              </b-form-input>
            </b-col>
          </b-row>

          <b-row class="form-row">
            <b-col cols="4"><p class="form-label">House number:</p></b-col>
            <b-col cols="8">
              <b-form-input
                  v-model="form.houseNumber"
                  type="number"
                  placeholder="Enter house number">
              </b-form-input>
            </b-col>
          </b-row>

          <b-row class="form-row">
            <b-col cols="4"><p class="form-label">City:</p> </b-col>
            <b-col cols="8">
              <b-form-input
                  id="city"
                  v-model="form.city"
                  placeholder="Enter city">
              </b-form-input>
            </b-col>
          </b-row>

          <b-row class="form-row">
            <b-col cols="4"><p class="form-label">Postal code:</p> </b-col>
            <b-col cols="8">
              <b-form-input
                  id="zip"
                  v-model="form.postcode"
                  type="number"
                  placeholder="Enter postal code">
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
  name: 'UserForm',
  data() {
    return {
      showDismissibleAlert: false,
      showDismissibleAlertError: false

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
        this.showDismissibleAlertError=true;
      })
    }
    }
}
</script>