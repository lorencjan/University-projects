<template>
<div>
  <NavbarFinal></NavbarFinal>
      <b-container class="edit_hard-copy-borrowings">
      <b-form @submit.prevent="submit">
        <b-row>
          <b-col>
            <h1 class="display-4" >Borrowing on book {{this.borrowing.exemplar.titleName}}</h1>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <label class="typo__label required">State</label>
            <multiselect
                v-model="borrowing.state"
                :options="options"
                placeholder="Select state"
                label="text"
                track-by="value"
            >
            </multiselect>
            <b-form-invalid-feedback>
              Reservation state can not be empty!
            </b-form-invalid-feedback>
          </b-col>
          <b-col>
            <label class="typo__label required">Reader</label>
            <multiselect
                v-model="borrowing.reader"
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
        <b-row class="mt-3">
          <b-col>
            <b-form-group
                id="start-date-label"
                label="Borrowed from:"
                label-for="dateOfBorrowStart"
            >
            <div class="box">
              <section>
                <date-picker v-model="borrowing.dateOfBorrowStart" value-type="timestamp" format="DD.MM.YYYY"></date-picker>
              </section>
            </div>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="start-date-label"
                label="Borrowed until:"
                label-for="dateOfBorrowStart"
            >
            <div class="box">
              <section>
                <date-picker v-model="borrowing.dateOfBorrowEnd" value-type="timestamp" format="DD.MM.YYYY"></date-picker>
              </section>
            </div>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="return-date-label"
                label="Return date:"
                label-for="borrowing.returnDate"
                v-if="borrowing.state.value === 'RETURNED'"
            >
            <div class="box">
              <section>
                <date-picker v-model="borrowing.returnDate" value-type="timestamp" format="DD.MM.YYYY"></date-picker>
              </section>
            </div>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
        </b-row>
        <b-col class="text-center mt-4">
          <b-button @click="submit" variant="primary">Update</b-button>
        </b-col>
      </b-form>
    </b-container>
    <fineBorrowTable
            endpointGet="/fines/"
            endpointEdit="/edit_fines/"
            endpointDel="/fines/"
            type="fines"
            :fields="fieldsFines"
            sortBy="state"
            :parse="parseFines"
            tableId="tableBorrow"
            :borrowingId="borrowing.id"
        >
        </fineBorrowTable>
    </div>
</template>

<script>
import ApiConnect from "@/services/ApiConnect";
import DatePicker from 'vue2-datepicker';
import 'vue2-datepicker/index.css';
import NavbarFinal from "@/components/main_page/NavbarFinal";
import Multiselect from "vue-multiselect";
import fineBorrowTable from "@/components/borrowing_page/finesBorrowingTable";

export default {
    components: {
      Multiselect,
      DatePicker,
      fineBorrowTable,
      NavbarFinal
    },
    data() {
        return {
          borrowing: {},
          isMagazine: false,
          isBook: false,
          isReturned: false,
          exemplar_parent: {},
          magazine: {},
          exemplar: {},
          readers: [],
          fieldsFines: [
            {key: 'amount', sortable: true},
            {key: 'state', sortable: true},
            {key: 'borrowing_name', sortable: true},
            {key: 'reader', sortable: true},
            {key: 'pay', sortable: false},
            {key: 'delete', sortable: false},
          ],
          options: [
            { value: 'ACTIVE', text: 'Active' },
            { value: 'CAN_NOT_PROLONG', text: 'Can not prolong' },
            { value: 'TO_RETURN', text: 'To return' },
            { value: 'RETURNED', text: 'Returned' }
          ]
        }
    },
    methods: {
        getBorrowing(id){
          ApiConnect.get('/hard-copy-borrowings/'+id).then((response) =>
          {
            this.borrowing = response.data;
            this.options.forEach(state => {
              if(state.value === response.data.state) this.borrowing.state = state
            })
            this.borrowing.returnDate != null ? this.setReturnDate(true) : this.setReturnDate(false);
            if (this.borrowing.exemplar.book == null)
            {
              this.isMagazine = true;
              this.isBook = false;
              this.magazine = this.borrowing.exemplar.magazine;
            }
            else
            {
              this.isMagazine = false;
              this.isBook = true;
              this.book = this.borrowing.exemplar.book;
            }
          }).catch((error) =>
          {
            console.log(error);
          })
        },
        setReturnDate(bool)
        {
          this.isReturned = bool;
        },
        submit(){
          let saveFormState = this.borrowing.state;
          this.borrowing.state = this.borrowing.state.value
          ApiConnect.put('/hard-copy-borrowings', this.borrowing).then((response) =>{
            this.makeToast('Borrowing '+this.borrowing.id+' has been updated successfully.')
            this.borrowing.state = saveFormState;
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
    check_borrowing_form(){

    },
    getReaders(){
      ApiConnect.get('/readers/').then(resp =>{
        this.readers = resp.data
      }).catch(error => console.log(error));
    },
    parseFines(data){
      data.forEach(fine => {
        fine.borrowing_name = '';
        fine.reader = ''})
      data.forEach(function (fine){
        ApiConnect.get('/hard-copy-borrowings/'+fine.borrowingId).then((response) => {
          fine.borrowing_name = response.data.exemplar.titleName;
          fine.reader = response.data.reader.fullname;
            }
        )
      })
      return data
    },
  },
  created() {
    this.getBorrowing(this.$route.params.id);
    this.getReaders();
  },
  computed: {
    readerSelection: {
      get() {
        if(this.borrowing.reader !== undefined){
          this.reader = this.borrowing.reader
        }
        return this.selectedReader
      },
      set(newValue) {
        this.borrowing.reader = undefined
        this.reader = newValue.reader
        this.selectedReader = newValue
      }
    },
  }
}
</script>

<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
<style scoped>
.edit_hard-copy-borrowings{
  color: black;
  text-align: left;
}
.preview{
  color: #24433e;
  box-shadow: 0 6px 10px rgba(0,0,0,0), 0 0 6px rgba(0,0,0,0);
  transition: .3s transform cubic-bezier(.155,1.105,.295,1.12),.3s box-shadow,.3s -webkit-transform cubic-bezier(.155,1.105,.295,1.12);
}
.preview:hover{
  transform: scale(1.05);
  box-shadow: 0 10px 20px rgba(0,0,0,.12), 0 4px 8px rgba(0,0,0,.06);
}
</style>