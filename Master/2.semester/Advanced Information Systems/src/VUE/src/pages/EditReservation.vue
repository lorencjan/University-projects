<template>
  <div>
    <NavbarFinal></NavbarFinal>
    <b-container class="edit-reservation-page">
      <b-row v-if="this.reservation.exemplar !== undefined">
        <b-col>
          <h1 class="display-4" >Reservation on book {{this.reservation.exemplar.titleName}}</h1>
        </b-col>
      </b-row>
      <b-form @submit.prevent="submit">
        <b-row>
          <b-col>
            <b-form-group
                id="dateFrom-label"
                label="Reserved form:"
                label-class="required"
                label-for="dateFrom"
            >
              <date-picker
                  ref="dateFrom"
                  id="dateFrom"
                  v-model="reservation.dateFrom"
                  placeholder="Enter date from of reservation start"
                  required
              ></date-picker>
              <b-form-invalid-feedback>
                Reservation has to have start date!
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="dateUntil-label"
                label="Reserved until:"
                label-class="required"
                label-for="dateUntil"
            >
              <date-picker
                  ref="dateUntil"
                  id="dateUntil"
                  v-model="reservation.dateUntil"
                  placeholder="Enter date until end of reservation"
                  required
              ></date-picker>
              <b-form-invalid-feedback>
                Reservation has to have end date!
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="state-label"
                label="State:"
                label-class="required"
                label-for="state"
            >
              <multiselect
                  ref="state"
                  id="state"
                  v-model="reservation.state"
                  :options="options"
                  label="text"
                  track-by="text"
                  placeholder="Enter state of reservation"
                  required
              ></multiselect>
              <b-form-invalid-feedback>
                Reservation state can not be empty!
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <label class="typo__label required">Select Book or Magazine</label>
            <multiselect
                v-model="bookSelection"
                ref="bookSelection"
                :options="data"
                :custom-label="nameWithType"
                placeholder="Select book or magazine"
                label="name"
                track-by="name"
            >

            </multiselect>
            <b-form-invalid-feedback>
              You have to select book!
            </b-form-invalid-feedback>
          </b-col>
          <b-col>
            <label class="typo__label required">Select Exemplar</label>
            <multiselect
                v-model="exemplarSelection"
                ref="exemplarSelection"
                :options="exemplars"
                placeholder="Select exemplar"
                label="state"
                track-by="id"
            >

            </multiselect>
            <b-form-invalid-feedback>
              You have to select exemplar!
            </b-form-invalid-feedback>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <label class="typo__label required">Select reader</label>
            <multiselect
                v-model="reservation.reader"
                ref="readerSelection"
                :options="readers"
                placeholder="Select reader"
                label="fullname"
                track-by="id"
            >

            </multiselect>
            <b-form-invalid-feedback>
              You have to select reader!
            </b-form-invalid-feedback>
          </b-col>
        </b-row>
        <b-row v-if="this.reservation.reader !== undefined">
          <b-col>
            <p>
              Reserved for {{ reservation.reader.fullname }}
            </p>
          </b-col>
        </b-row>
        <b-row v-if="showError">
          <p style="color: red">{{errorMessage}} </p>
        </b-row>
        <b-row v-if="this.$route.params.id != 0" align-h="center">
          <b-col class="text-center mt-4" cols="2">
            <b-button @click="submit" variant="primary">Update</b-button>
          </b-col>
          <b-col class="text-center mt-4" cols="2">
            <b-button @click="convertToBorrowing" variant="secondary">Borrow</b-button>
          </b-col>
        </b-row>
        <b-row v-if="this.$route.params.id == 0">
          <b-col class="text-center mt-4">
            <b-button @click="create" variant="success">Create</b-button>
          </b-col>
        </b-row>
      </b-form>
    </b-container>
  </div>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";
import Multiselect from "vue-multiselect";
import DatePicker from "vue2-datepicker";
import NavbarFinal from "@/components/main_page/NavbarFinal";
import reservationList from "@/pages/ReservationList";

export default {
  name: "EditReservation",
  components: {
    Multiselect,
    DatePicker,
    NavbarFinal
  },
  data () {
    return {
      reservation: {},
      books: [],
      magazines: [],
      data: [],
      selectedBook: undefined,
      exemplars: [],
      selectedExemplar: undefined,
      readers: [],
      showError: false,
      errorMessage: '',
      selected: null,
      options: [
        { value: 'ACTIVE', text: 'Active' },
        { value: 'PICK_UP', text: 'Can pick up' },
        { value: 'NOT_ACTIVE', text: 'Not active' }
      ]
    }
  },
  methods: {
    getReservation(id){
      ApiConnect.get('/reservations/'+id).then(response => {
        this.reservation = response.data
        this.reservation.dateFrom = new Date(this.reservation.dateFrom)
        this.reservation.dateUntil = new Date(this.reservation.dateUntil)

        if (this.reservation.state == 'ACTIVE')  this.reservation.state = { value: 'ACTIVE', text: 'Active' } ;
        if (this.reservation.state == 'PICK_UP')  this.reservation.state = { value: 'PICK_UP', text: 'Can pick up' } ;
        if (this.reservation.state == 'NOT_ACTIVE')  this.reservation.state = { value: 'NOT_ACTIVE', text: 'Not active' } ;

        if(response.data.exemplar.book !== undefined) {
          ApiConnect.get('/books/'+response.data.exemplar.book.id).then(resp =>
            this.selectedBook = resp.data
          )
        }else{
          ApiConnect.get('/magazines/'+response.data.exemplar.magazine.id).then(resp =>
              this.selectedBook = resp.data
          )
        }

      })
    },
    check_reservation_form(){
      let form_check_error = false;
      if (! this.reservation.dateFrom){
        this.$refs['dateFrom'].state = false;
        this.$refs['dateFrom'].value = "";
        form_check_error = true;
      }
      if (! this.reservation.dateUntil){
        this.$refs['dateUntil'].state = false;
        this.$refs['dateUntil'].value = "";
        form_check_error = true;
      }
      if (this.reservation.dateUntil <= this.reservation.dateFrom){
        this.showError = true;
        this.errorMessage = "Reservation can not end before/same day as starting";
        form_check_error = true;
      }
      if (! this.reservation.state){
        this.$refs['state'].state = false;
        this.$refs['state'].value = "";
        form_check_error = true;
      }
      if (! this.bookSelection){
        this.$refs['bookSelection'].state = false;
        this.$refs['bookSelection'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Book field can not be empty.';
        return form_check_error;
      }
      if (! this.exemplarSelection ){
        this.$refs['exemplarSelection'].state = false;
        this.$refs['exemplarSelection'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Exemplar field can not be empty.';
        return form_check_error;
      }

      if (! this.reservation.reader ){
        this.$refs['readerSelection'].state = false;
        this.$refs['readerSelection'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Reader field can not be empty.';
        return form_check_error;
      }
      return form_check_error;
    },
    submit(){
      if ( this.check_reservation_form()) return;
      this.reservation.state = this.reservation.state.value;
      ApiConnect.put('/reservations', this.reservation).then((response) =>{
        this.showError = false;
        if (this.reservation.state == 'ACTIVE')  this.reservation.state = { value: 'ACTIVE', text: 'Active' } ;
        if (this.reservation.state == 'PICK_UP')  this.reservation.state = { value: 'PICK_UP', text: 'Can pick up' } ;
        if (this.reservation.state == 'NOT_ACTIVE')  this.reservation.state = { value: 'NOT_ACTIVE', text: 'Not active' } ;

        this.makeToast('Reservation on book'+this.reservation.exemplar.titleName +' has been updated successfully.')
      }).catch(error => {
        console.log(error)
      })
    },
    create(){
      if (this.check_reservation_form()) return;
      this.reservation.state = this.reservation.state.value;
      ApiConnect.post('/reservations', this.reservation).then((response) =>{
        this.showError = false;
        this.makeToast('Reservation on book '+this.reservation.exemplar.titleName+' has been created successfully.')
        ApiConnect.get('/reservations/').then(resp =>{
          this.$router.push('/edit_reservations/'+(resp.data[resp.data.length -1].id+1))
        })
      }).catch(error => {
        console.log(error)
      })

    },
    makeToast(text) {
      this.$bvToast.toast(text, {
        title: 'Library',
        variant: 'success',
        autoHideDelay: 5000,
      })
    },
    nameWithType ({ name, issn, isbn }) {
      if (issn != undefined) return `${name} — [magazine]`
      if (isbn != undefined) return `${name} — [book]`
    },
    getData(){
      ApiConnect.get('/books/').then(resp =>{
        this.books = resp.data;
        ApiConnect.get('/magazines/').then(response =>{
          this.magazines = response.data;
          this.data = this.data.concat(this.books,this.magazines);
        })
      })
    },
    getReaders(){
      ApiConnect.get('/readers/').then(resp =>{
        this.readers = resp.data
      }).catch(error => console.log(error));
    },
    convertToBorrowing(){
      let borrow = {};
      borrow.id = 0;
      borrow.dateOfBorrowStart = new Date().getTime();
      borrow.reader = this.reservation.reader;
      borrow.state = 'ACTIVE';
      borrow.exemplar = this.reservation.exemplar;
      borrow.exemplar.availability = false;
      borrow.dateOfBorrowEnd = new Date().getTime() + new Date(this.reservation.exemplar.borrowPeriod).getTime();
      borrow.returnDate = null
      console.log(borrow)
      ApiConnect.delete('/reservations/'+this.reservation.id).then(resp => {
        console.log(resp)
        ApiConnect.post('/hard-copy-borrowings',borrow).then(resp => {
          console.log(resp)

        });
      });

      this.$router.push('/edit_borrowings');

    }
  },
  created() {
    if(this.$route.params.id == 0){
      this.reservation = {
        id: 0,
        dateFrom: new Date(),
        dateUntil: new Date(),
        exemplar: undefined,
        state: { value: 'ACTIVE', text: 'Active' }
      }
    }else {
      this.getReservation(this.$route.params.id);
    }
    this.getData();
    this.getReaders();
  },
  computed: {
    bookSelection: {
      get() {
        if(this.reservation.exemplar !== undefined){
          this.exemplars = this.selectedBook.hardCopyExemplars
        }
        return this.selectedBook
      },
      set(newValue) {
        this.reservation.exemplar = undefined
        this.exemplars = newValue.hardCopyExemplars
        this.selectedBook = newValue
      }
    },
    exemplarSelection: {
      get() {
        return this.reservation.exemplar
      },
      set(newValue) {
        this.reservation.exemplar = newValue
      }
    }
  }
}
</script>

<style scoped>
.edit-reservation-page{
  color: black;
  text-align: left;
}
</style>