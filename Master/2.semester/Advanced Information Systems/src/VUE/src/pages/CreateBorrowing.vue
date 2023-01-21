<template>
  <div>
    <NavbarFinal></NavbarFinal>
    <b-container class="edit-borrow-page">
      <b-form @submit.prevent="submit">
        <b-row>
          <b-col>
            <b-form-group
                id="dateOfBorrowStart-label"
                label="Borrowed form:"
                label-class="required"
                label-for="dateOfBorrowStart"
            >
              <date-picker
                  ref="dateOfBorrowStart"
                  id="dateOfBorrowStart"
                  v-model="borrow.dateOfBorrowStart"
                  placeholder="Enter date from of borrow start"
                  required
              ></date-picker>
              <b-form-invalid-feedback>
                borrow has to have start date!
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="dateOfBorrowEnd-label"
                label="Reserved until:"
                label-class="required"
                label-for="dateOfBorrowEnd"
            >
              <date-picker
                  ref="dateOfBorrowEnd"
                  id="dateOfBorrowEnd"
                  v-model="borrow.dateOfBorrowEnd"
                  placeholder="Enter date until end of borrow"
                  required
              ></date-picker>
              <b-form-invalid-feedback>
                borrow has to have end date!
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
                  v-model="borrow.state"
                  :options="options"
                  label="text"
                  track-by="text"
                  placeholder="Enter state of borrow"
                  required
              ></multiselect>
              <b-form-invalid-feedback>
                borrow state can not be empty!
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
                v-model="borrow.reader"
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
        <b-row v-if="this.borrow.reader !== undefined">
          <b-col>
            <p>
              Reserved for {{ borrow.reader.fullname }}
            </p>
          </b-col>
        </b-row>
        <b-row v-if="showError">
          <p style="color: red">{{errorMessage}} </p>
        </b-row>
        <b-row>
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

export default {
  name: "CreateBorrowing",
  components: {
    Multiselect,
    DatePicker,
    NavbarFinal
  },
  data () {
    return {
      borrow: {},
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
        { value: 'CAN_NOT_PROLONG', text: 'Can not prolong' },
        { value: 'TO_RETURN', text: 'To return' },
        { value: 'RETURNED', text: 'Returned' }
      ]
    }
  },
  methods: {
    getBorrow(id){
      ApiConnect.get('/borrows/'+id).then(response => {
        this.borrow = response.data
        this.borrow.dateOfBorrowStart = new Date(this.borrow.dateOfBorrowStart)
        this.borrow.dateOfBorrowEnd = new Date(this.borrow.dateOfBorrowEnd)
        this.options.forEach(state => {
          if(state.value === response.data.state) this.borrow.state = state
        })

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
    check_borrow_form(){
      let form_check_error = false;
      if (! this.borrow.dateOfBorrowStart){
        this.$refs['dateOfBorrowStart'].state = false;
        this.$refs['dateOfBorrowStart'].value = "";
        form_check_error = true;
      }
      if (! this.borrow.dateOfBorrowEnd){
        this.$refs['dateOfBorrowEnd'].state = false;
        this.$refs['dateOfBorrowEnd'].value = "";
        form_check_error = true;
      }
      if (this.borrow.dateOfBorrowEnd <= this.borrow.dateOfBorrowStart){
        this.showError = true;
        this.errorMessage = "borrow can not end before/same day as starting";
        form_check_error = true;
      }
      if (! this.borrow.state){
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

      if (! this.borrow.reader ){
        this.$refs['readerSelection'].state = false;
        this.$refs['readerSelection'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Reader field can not be empty.';
        return form_check_error;
      }
      return form_check_error;
    },
    create(){
      if (this.check_borrow_form()) return;
      this.borrow.state = this.borrow.state.value;
      this.borrow.exemplar.availability = false;
      this.borrow.returnDate = null
      ApiConnect.post('/hard-copy-borrowings', this.borrow).then((response) =>{
        this.showError = false;
        this.makeToast('borrow on book '+this.borrow.exemplar.titleName+' has been created successfully.')
        ApiConnect.get('/hard-copy-borrowings/').then(resp =>{
          this.$router.push('/edit_hard-copy-borrowings/'+(resp.data[resp.data.length -1].id+1))
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
    }
  },
  created() {
    this.borrow = {
      id: 0,
      dateOfBorrowStart: new Date(),
      dateOfBorrowEnd: new Date(),
      exemplar: undefined,
      state: { value: 'ACTIVE', text: 'Active' }
    }
    this.getData();
    this.getReaders();
  },
  computed: {
    bookSelection: {
      get() {
        if(this.borrow.exemplar !== undefined){
          this.exemplars = this.selectedBook.hardCopyExemplars
        }
        return this.selectedBook
      },
      set(newValue) {
        this.borrow.exemplar = undefined
        this.exemplars = newValue.hardCopyExemplars
        this.selectedBook = newValue
      }
    },
    exemplarSelection: {
      get() {
        return this.borrow.exemplar
      },
      set(newValue) {
        this.borrow.exemplar = newValue
      }
    }
  }
}
</script>

<style scoped>
.edit-borrow-page{
  color: black;
  text-align: left;
}
</style>