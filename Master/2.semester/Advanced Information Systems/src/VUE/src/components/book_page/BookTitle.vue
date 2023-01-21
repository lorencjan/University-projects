<template>
  <div id="book-title" class="text-center">
    <b-alert class="mt-2" v-model="showDismissibleAlertBorrow" variant="success" dismissible>
      {{ alertMessage }}<br>
    You can download it <a :href="filePath">here.</a>
    </b-alert>
    <b-alert class="mt-2" v-model="showDismissibleAlertErrorBorrow" variant="danger" dismissible>
      {{alertMessage}}</b-alert>
    <b-row>
      <b-col v-if="img !== null ">
        <b-img class="book-cover" :src="img" alt="Book cover" ></b-img>
      </b-col>
      <b-col v-else class="my-4">
        <font-awesome-icon icon="fa-solid fa-book" size="10x" class="img-cover"/>
      </b-col>
    </b-row>
    <b-row class="text-left">
      <b-col v-if="loggedUser" offset-md="2">
        <b-button v-if="hardCopies.length > 0" @click="showModalReservation" variant="info" class="mr-2"> Reserve </b-button>
        <b-button v-if="hasElectronicCopy" @click="borrowEcopy" variant="success">Borrow electronic copy</b-button>
      </b-col>
      <b-col v-if="loggedEmployee && id !== undefined" offset-md="2">
        <router-link :to="{path: '/edit_books/'+id}"><b-button variant="secondary" class="mr-2" v-if="format === 'Book' "> Edit </b-button></router-link>
        <router-link :to="{path: '/edit_magazines/'+id}"><b-button variant="secondary" class="mr-2" v-if="format === 'Magazine' "> Edit </b-button></router-link>
      </b-col>
    </b-row>

    <!--MODAL RESERVATION--->
    <b-modal ref="new-reservation" hide-footer title="Create new reservation">
      <div class="d-block text-center">
        <b-alert class="mt-2" v-model="showDismissibleAlert" variant="success" dismissible>
          {{ alertMessage }}</b-alert>
        <b-alert class="mt-2" v-model="showDismissibleAlertError" variant="danger" dismissible>
          {{alertMessage}}</b-alert>
        <b-row>
          <b-col cols="5">
            <p style="text-align: left" class="form-label required" >Select start date:</p>
          </b-col>
          <b-col>
            <date-picker id="example-datepickerRes"
                        monday-first
                        v-model="dateFrom"
                        placeholder="Choose new date"
                        class="mb-1 mt-1 ml-0"
            ></date-picker>
          </b-col>
        </b-row>

        <b-row>
          <b-col cols="5">
            <p style="text-align: left" class="form-label required" >Select end date:</p>
          </b-col>
          <b-col>
            <date-picker id="example-datepickerRes2"
                        monday-first
                        v-model="dateTo"
                        placeholder="Choose new date"
                        class="mb-1 mt-1 ml-0"
            ></date-picker>
          </b-col>
        </b-row>
        <b-row>
          <b-col cols="5">
            <p style="text-align: left" class="form-label required" >Select hard copy:</p>
          </b-col>
          <b-col>
            <multiselect v-if="hardCopies"
                           v-model="selectedHardCopy"
                           :options="hardCopies"
                           label="state"
                           track-by="id">
            </multiselect>
          </b-col>
        </b-row>

      </div>
      <b-button class="mt-3" variant="outline-primary" block @click="makeReservation">Save</b-button>
    </b-modal>


    <b-row align-h="center" class="mt-3">
      <b-col cols="4" class="text-right text-dark h5">
        Format
      </b-col>
      <b-col cols="4" class="text-left text-dark">
        {{ format }}
      </b-col>
    </b-row>
    <b-row align-h="center" class="mt-3">
      <b-col cols="4" class="text-right text-dark h5">
        Publisher
      </b-col>
      <b-col cols="4" class="text-left text-dark">
        {{ publisher }}
      </b-col>
    </b-row>
    <b-row align-h="center" class="mt-3" v-if="released !== undefined">
      <b-col cols="4" class="text-right text-dark h5">
        Released
      </b-col>
      <b-col cols="4" class="text-left text-dark">
        {{ released | formatDate }}
      </b-col>
    </b-row>
    <b-row align-h="center" class="mt-3" v-if="pages !== undefined">
      <b-col cols="4" class="text-right text-dark h5">
        Pages
      </b-col>
      <b-col cols="4" class="text-left text-dark">
        {{ pages }}
      </b-col>
    </b-row>
  </div>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";
import DatePicker from "vue2-datepicker";
import Multiselect from "vue-multiselect";

export default {
  name: "BookTitle",
  components: {
    DatePicker,
    Multiselect
  },
  props: {
    img: undefined,
    format: String,
    publisher: String,
    released: Date,
    pages: Number,
    hasElectronicCopy: Boolean,
    hardCopies: [],
    electronicCopies: [],
    id: Number,
  },
  data (){
    return {
      showDismissibleAlert: false,
      showDismissibleAlertError: false,
      showDismissibleAlertBorrow: false,
      showDismissibleAlertErrorBorrow: false,
      alertMessage: '',
      dateFrom: new Date(),
      dateTo: new Date(),
      selectedHardCopy: null,
      filePath: ''
    }
  },
  methods: {
    showModalReservation() {
      this.$refs['new-reservation'].show()
    },
    showModalBorrow() {
      this.$refs['new-borrow'].show()
    },
    makeReservation(){
      let reservation = {}
      reservation.id = 0;

      let today = new Date();
      today.setHours(23,59,59,59);

      let startDate = new Date(this.dateFrom);
      startDate.setHours(23,59,59,59);

      let endDate = new Date(this.dateTo);
      endDate.setHours(23,59,59,59);

      if (startDate < today){
        this.showDismissibleAlertError = true;
        this.showDismissibleAlert = false;
        this.alertMessage = "Reservation can not start before today";
      }
      else if (startDate > endDate){
        this.showDismissibleAlertError = true;
        this.showDismissibleAlert = false;
        this.alertMessage = "Reservation can not end before starting.";
      }
      else if (startDate < endDate){
        reservation.state = 'ACTIVE';
        reservation.dateFrom = startDate;
        reservation.dateUntil = endDate;
        reservation.exemplar = this.selectedHardCopy;
        let user = {};
        user.id = parseInt(localStorage.getItem('id'));
        reservation.reader= user;

        ApiConnect.post('/reservations', reservation).then(response => {
          this.showDismissibleAlert = true;
          this.showDismissibleAlertError = false;
          this.alertMessage = "Reservation succesfully created.";
        }).catch(error => {
          this.showDismissibleAlertError = true;
          this.showDismissibleAlert = false;
          this.alertMessage = "Error while creating reservation.";
        })
      }
      else {
        this.showDismissibleAlertError = true;
        this.showDismissibleAlert = false;
        this.alertMessage = "Reservation can not end the same day as starting.";
      }
    },

    async borrowEcopy(){
      if (confirm("Do you really want to borrow electronic copy?")){
        let borrowing = {};
        borrowing.id = 0;
        borrowing.dateOfBorrowStart = new Date();

        let ecopy = this.electronicCopies[0];
        ApiConnect.get('electronic-copy-exemplars/' + ecopy.id).then((response) => {
          this.filePath = response.data.filePath;
        })

        let borrowPeriod = ecopy.borrowPeriod;
        let dateTo = new Date();

        dateTo.setDate(dateTo.getDate() + borrowPeriod);
        borrowing.dateOfBorrowEnd = dateTo ;
        borrowing.borrowCounter = 1;
        borrowing.electronicCopy = ecopy;

        let user = {};
        user.id = parseInt(localStorage.getItem('id'));

        borrowing.reader = user;
        ApiConnect.post('/electronic-copy-borrowings', borrowing).then(response => {
          this.showDismissibleAlertBorrow = true;
          this.showDismissibleAlertErrorBorrow = false;
          this.alertMessage = "Succesfully borrowed electronic copy.";

        }).catch(error => {
          this.showDismissibleAlertError = true;
          this.showDismissibleAlert = false;
          this.alertMessage = "Error while borrowing electronic copy.";
        })
      }
    }
  },
  computed: {
    hardCopiesOptions: function(){
      let exemplars = []
      let counter = 1;
      for (let ex of this.hardCopies){
        let selectOption = {};
        selectOption.value = ex;
        selectOption.text = 'Exemplar ' + counter + '( state: '+ ex.state.toLowerCase() +')';
        if (counter === 1) this.selectedHardCopy=ex;
        counter += 1
        exemplars.push(selectOption);
      }
      return exemplars;
    },
    loggedUser: function (){
      if (localStorage.getItem('role') === "\"READER\"") return true;
      return false;
    },
    loggedEmployee: function (){
      if (localStorage.getItem('role') == "\"EMPLOYEE\"" || localStorage.getItem('role') == "\"ADMIN\"") return true;
      return false;
    }

  },
  created() {
    console.log(this.img);
  }

}
</script>

<style scoped>
.img-cover{
  color:#24433e;
}
</style>