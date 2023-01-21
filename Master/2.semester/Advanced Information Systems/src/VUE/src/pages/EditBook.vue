<template>
  <div>
    <NavbarFinal></NavbarFinal>
    <b-container class="edit-book-page">
      <b-row class="text-center">
        <b-col>
          <font-awesome-icon icon="fa-solid fa-display" size="4x" v-b-modal.modal-preview type="button" class="preview"/>
        </b-col>
      </b-row>
      <b-row class="text-center mb-4">
        <b-col>
          preview book page
        </b-col>
      </b-row>
      <b-form @submit.prevent="submit">
        <b-row>
          <b-col>
            <b-form-group
                id="title-label"
                label="Title:"
                label-class="required"
                label-for="title"
            >
              <b-form-input
                  ref="title"
                  id="title"
                  v-model="book.name"
                  type="text"
                  placeholder="Enter book title"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Title field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="language-label"
                label="Language:"
                label-class="required"
                label-for="lang"
            >
              <b-form-input
                  ref="lang"
                  id="lang"
                  v-model="book.language"
                  type="text"
                  placeholder="Enter book Language"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Laguage field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <b-form-group
                id="publisher-label"
                label="Publisher:"
                label-class="required"
                label-for="publisher"
            >
              <b-form-input
                  ref="publisher"
                  id="publisher"
                  v-model="book.publisher"
                  type="text"
                  placeholder="Enter book publisher"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Publisher field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="isbn-label"
                label="ISBN:"
                label-class="required"
                label-for="isbn"
            >
              <b-form-input
                  ref="isbn"
                  id="isbn"
                  v-model="book.isbn"
                  type="number"
                  placeholder="Enter book isbn"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                ISBN field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="publicationDate-label"
                label="Publication date:"
                label-class="required"
                label-for="lang"
            >
              <date-picker
                  ref="publicationDate"
                  id="publicationDate"
                  v-model="book.publicationDate"
                  type="date"
                  placeholder="Enter date when book was published"
                  required
              ></date-picker>
              <b-form-invalid-feedback>
                Publication date field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <b-form-group
                id="publicationNumber-label"
                label="Publication number:"
                label-class="required"
                label-for="publicationNumber"
            >
              <b-form-input
                  ref="publicationNumber"
                  id="publicationNumber"
                  v-model="book.publicationNumber"
                  type="number"
                  placeholder="Enter book publication number"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Publication number field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="pages-label"
                label="Pages:"
                label-class="required"
                label-for="pages"
            >
              <b-form-input
                  ref="pages"
                  id="pages"
                  v-model="book.pages"
                  type="number"
                  placeholder="Enter number of book pages"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Number of book pages field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <label class="typo__label required" for="authors">Authors</label>
            <multiselect
                v-model="book.authors"
                id="authors"
                ref="authors"
                label="name"
                track-by="name"
                placeholder="Type to search"
                open-direction="bottom"
                :options="authors"
                :multiple="true"
                :searchable="true"
                :loading="isLoading"
                :internal-search="false"
                :clear-on-select="false"
                :close-on-select="false"
                :options-limit="300"
                :limit="3"
                :limit-text="limitTextAuthor"
                :max-height="600"
                :show-no-results="false"
                :hide-selected="true"
                @search-change="asyncFindAuthor"
            >
              <!--
              <template slot="tag" slot-scope="{ option, remove }">
                <b-badge pill variant="primary">{{ option.name }} <b-icon icon="x" scale="2" @click="remove(option)" type="button"></b-icon></b-badge>
              </template>-->
              <template slot="clear" slot-scope="props">
                <div class="multiselect__clear" v-if="book.authors" @mousedown.prevent.stop="clearAll(props.search)"></div>
              </template>
              <template slot="noResult">
                <span>Oops! No authors found. Consider changing the search query.</span>
              </template>
            </multiselect>
          </b-col>
          <b-col>
            <label class="typo__label required">Genres</label>
            <multiselect
                v-model="book.genres"
                ref="genres"
                :options="genres"
                :multiple="true"
                :close-on-select="false"
                :clear-on-select="false"
                :preserve-search="true"
                placeholder="Pick some"
                label="name"
                track-by="name"
                :preselect-first="true">
              <template slot="clear" slot-scope="props">
                <div class="multiselect__clear" v-if="book.genres" @mousedown.prevent.stop="clearAll(props.search)"></div>
              </template>
              <template slot="noResult">
                <span>Oops! No Genres found. Consider changing the search query.</span>
              </template>
            </multiselect>
          </b-col>
        </b-row>
        <b-row class="mt-3">
          <b-col cols="6" class="text-left"><label>Cover photo</label><br>
            <div style="position: initial">
              <b-form-file
                  id="coverPhotoInput"
                  @input="coverPhotoInputChange"
                  v-model="coverPhoto"
                  placeholder="Choose a file or drop it here..."
                  drop-placeholder="Drop file here..."
              ></b-form-file>
            </div>
          </b-col>
        </b-row>
        <b-row class="mt-3" v-if="this.$route.params.id != 0">
          <b-col cols="6" class="text-left">
            <label >Hard exemplars</label><br>
            <div class="dropdown-menu d-block" style="position: initial">
              <template v-for="hardCopy in book.hardCopyExemplars">
                <b-row>
                  <b-col cols="8">
                    <span v-if="hardCopy.availability === false" class="dropdown-item text-danger">
                      state: {{ hardCopy.state }}, not available
                    </span>
                    <span v-if="hardCopy.availability !== false" class="dropdown-item text-success">
                      state: {{ hardCopy.state }}, available
                    </span>
                  </b-col>
                  <b-col cols="1" class="text-center">
                    <font-awesome-icon
                        icon="fa-solid fa-pen-to-square"
                        class="text-info"
                        type="button"
                        @click="editHardCopy(hardCopy)"
                    />
                  </b-col>
                  <b-col cols="1" class="text-center">
                    <font-awesome-icon
                        icon="fa-regular fa-circle-xmark"
                        class="text-danger"
                        type="button"
                        @click="deleteHardCopy(hardCopy.id)"
                    />
                  </b-col>
                </b-row>

              </template>
            </div>
          </b-col>
          <b-col cols="6" class="text-left">
            <label >Electronic exemplars</label><br>
            <div class="dropdown-menu d-block" style="position: initial">
              <template v-for="Copy in book.electronicCopyExemplars">
                <b-row>
                  <b-col cols="9">
                    <span v-if="Copy.availability === false" class="dropdown-item text-danger">
                      state: {{ Copy.state }}, not available
                    </span>
                    <span v-if="Copy.availability !== false" class="dropdown-item text-success">
                      state: {{ Copy.state }}, available
                    </span>
                  </b-col>
                  <b-col cols="1" class="text-center">
                    <font-awesome-icon
                        icon="fa-solid fa-pen-to-square"
                        class="text-info"
                        type="button"
                        @click="editElectronicCopy(Copy)"
                    />
                  </b-col>
                  <b-col cols="1" class="text-center">
                    <font-awesome-icon
                        icon="fa-regular fa-circle-xmark"
                        class="text-danger"
                        type="button"
                        @click="deleteElectroCopy(Copy.id)"
                    />
                  </b-col>
                </b-row>
              </template>
            </div>
          </b-col>
        </b-row>
        <b-row class="mt-3" v-if="this.$route.params.id != 0">
          <b-col>
            <b-button variant="success" class="ml-4"  v-b-modal.modal-addHardCopy> Add </b-button>
          </b-col>
          <b-col>
            <b-button variant="success" class="ml-4"  v-b-modal.modal-addElectronicCopy> Add </b-button>
          </b-col>
        </b-row>
        <b-row class="mt-3">
          <b-col>
            <label>Description:</label>
            <b-form-textarea
                id="description"
                placeholder="Description"
                rows="8"
                v-model="book.description"
            ></b-form-textarea>
          </b-col>
        </b-row>
        <b-row v-if="showError">
           <p style="color: red">{{errorMessage}} </p>
        </b-row>
        <b-row v-if="this.$route.params.id != 0">
          <b-col class="text-center mt-4">
            <b-button @click="submit" variant="primary">Update</b-button>
          </b-col>
        </b-row>
        <b-row v-if="this.$route.params.id == 0">
          <b-col class="text-center mt-4">
            <b-button @click="create" variant="success">Create</b-button>
          </b-col>
        </b-row>
      </b-form>
    </b-container>
    <b-modal id="modal-preview" title="Preview" size="xl" hide-footer>
      <b-row>
        <b-col cols="4">
          <BookTitle
              :img="book.coverPhotoPath"
              format="book"
              :publisher="book.publisher"
              :released="new Date(book.publicationDate)"
              :pages="book.pages"
              :hardCopies="book.hardCopyExemplars"
              :electronicCopies="book.electronicCopyExemplars"
          >

          </BookTitle>
        </b-col>
        <b-col cols="8">
          <BookInfo
              :title="book.name"
              :publicationNumber="''+book.publicationNumber"
              :authors="book.authors"
              :isbn="book.isbn"
              :genres="book.genres"
              :description="book.description"
          >

          </BookInfo>
        </b-col>
      </b-row>
    </b-modal>
    <b-modal
        id="modal-addElectronicCopy"
        title="Add electronic copy"
        hide-footer
        ref="addElectronicCopy"
    >
      <b-form @submit.prevent="submit">
        <b-form-group
            id="maximumNumberOfExtension-label"
            label="Maximum number of extension:"
            label-class="required"
            label-for="maximumNumberOfExtension"
        >
          <b-form-input
              ref="maximumNumberOfExtension"
              id="maximumNumberOfExtension"
              v-model="electronicExtension"
              type="number"
              placeholder="Enter maximum number of borrowing extension"
              required
          ></b-form-input>
          <b-form-invalid-feedback>
            Maximum number of extention can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
            id="borrowPeriod-label"
            label="Borrow period:"
            label-class="required"
            label-for="borrowPeriod"
        >
          <b-form-input
              ref="borrowPeriod"
              id="borrowPeriod"
              v-model="electronicPeriod"
              type="number"
              placeholder="Enter maximus day that book could be borrowed"
              required
          ></b-form-input>
          <b-form-invalid-feedback>
            Borrow period can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
            id="electronicExampleFileInput-label"
            label="File:"
            label-class="required"
            label-for="electronicExampleFileInput"
        >
          <b-form-file
              ref="electronicExampleFileInput"
              id="electronicExampleFileInput"
              v-model="electronicExampleFile"
              placeholder="Choose a file or drop it here..."
              drop-placeholder="Drop file here..."
              required
          ></b-form-file>
          <b-form-invalid-feedback>
            Electronic example needs file.
          </b-form-invalid-feedback>
        </b-form-group>

        <b-button variant="success" class="ml-4" @click="addElectronicExample"> Add electronic copy </b-button>
      </b-form>
    </b-modal>
    <b-modal
        id="modal-addHardCopy"
        title="Add hard copy"
        hide-footer
        ref="addHardCopy"
    >
      <b-form @submit.prevent="submit">
        <b-form-group
            id="maximumNumberOfExtension-label"
            label="Maximum number of extension:"
            label-class="required"
            label-for="maximumNumberOfExtension"
        >
          <b-form-input
              ref="maximumNumberOfExtension"
              id="maximumNumberOfExtension"
              v-model="hardExtension"
              type="number"
              placeholder="Enter maximum number of borrowing extension"
              required
          ></b-form-input>
          <b-form-invalid-feedback>
            Maximum number of extention can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
            id="borrowPeriod-label"
            label="Borrow period:"
            label-class="required"
            label-for="borrowPeriod"
        >
          <b-form-input
              ref="borrowPeriod"
              id="borrowPeriod"
              v-model="hardPeriod"
              type="number"
              placeholder="Enter maximus day that book could be borrowed"
              required
          ></b-form-input>

          <b-form-invalid-feedback>
            Borrow period can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
            id="state-label"
            label="Books state:"
            label-for="state"
        >
          <multiselect
              ref="state"
              id="state"
              v-model="hardState"
              :options = "options"
              track-by="text"
              label="text"
              type="text"
              placeholder="Enter exemplar state"
              required
          ></multiselect>

          <b-form-invalid-feedback>
            Maximumu number of extention can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-button variant="success" class="ml-4" @click="addHardExample"> Add hard copy </b-button>
      </b-form>
    </b-modal>

    <b-modal
        id="modal-updateHardCopy"
        title="Edit hard copy"
        hide-footer
        ref="updateHardCopy"
    >
      <b-form @submit.prevent="submit">
        <b-form-group
            id="maximumNumberOfExtensionUpdate-label"
            label="Maximum number of extension:"
            label-class="required"
            label-for="maximumNumberOfExtensionUpdate"
        >
          <b-form-input
              ref="maximumNumberOfExtensionUpdate"
              id="maximumNumberOfExtensionUpdate"
              v-model="hardCopy.maximumNumberOfExtension"
              type="number"
              placeholder="Enter maximum number of borrowing extension"
              required
          ></b-form-input>
          <b-form-invalid-feedback>
            Maximum number of extention can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
            id="borrowPeriodUpdate-label"
            label="Borrow period:"
            label-class="required"
            label-for="borrowPeriodUpdate"
        >
          <b-form-input
              ref="borrowPeriodUpdate"
              id="borrowPeriodUpdate"
              v-model="hardCopy.borrowPeriod"
              type="number"
              placeholder="Enter maximus day that book could be borrowed"
              required
          ></b-form-input>

          <b-form-invalid-feedback>
            Borrow period can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
            id="stateUpdate-label"
            label="Books state:"
            label-for="stateUpdate"
        >
          <multiselect
              ref="stateUpdate"
              id="stateUpdate"
              v-model="hardCopy.state"
              :options="options"
              track-by="text"
              label="text"
              placeholder="Enter exemplar state"
              required
          ></multiselect>

          <b-form-invalid-feedback>
            Maximumu number of extention can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-button variant="info" class="ml-4" @click="editHardExample"> Update hard copy </b-button>
      </b-form>
    </b-modal>

    <b-modal
        id="modal-updateElectronicCopy"
        title="Update electronic copy"
        hide-footer
        ref="updateElectronicCopy"
    >
      <b-form @submit.prevent="submit">
        <b-form-group
            id="maximumNumberOfExtensionUpdate-label"
            label="Maximum number of extension:"
            label-class="required"
            label-for="maximumNumberOfExtensionUpdate"
        >
          <b-form-input
              ref="maximumNumberOfExtensionUpdate"
              id="maximumNumberOfExtensionUpdate"
              v-model="electronicCopy.maximumNumberOfExtension"
              type="number"
              placeholder="Enter maximum number of borrowing extension"
              required
          ></b-form-input>
          <b-form-invalid-feedback>
            Maximum number of extention can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>
        <b-form-group
            id="borrowPeriodUpdate-label"
            label="Borrow period:"
            label-class="required"
            label-for="borrowPeriodUpdate"
        >
          <b-form-input
              ref="borrowPeriodUpdate"
              id="borrowPeriodUpdate"
              v-model="electronicCopy.borrowPeriod"
              type="number"
              placeholder="Enter maximus day that book could be borrowed"
              required
          ></b-form-input>
          <b-form-invalid-feedback>
            Borrow period can not be empty.
          </b-form-invalid-feedback>
        </b-form-group>

        <b-button variant="info" class="ml-4" @click="editElectronicExample"> Update electronic copy </b-button>
      </b-form>
    </b-modal>


  </div>
</template>
<script>

import ApiConnect from "@/services/ApiConnect";
import Multiselect from "vue-multiselect";
import BookInfo from "@/components/book_page/BookInfo";
import BookTitle from "@/components/book_page/BookTitle";
import * as file from "../assets/js/file.js"
import DatePicker from "vue2-datepicker";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "EditBook",
  components: {
    BookInfo,
    BookTitle,
    Multiselect,
    DatePicker,
    NavbarFinal
  },
  data () {
    return {
      authors: [],
      isLoading: false,
      genres: [],
      isLoadingGenre: false,
      book: {},
      electronicExtension: 1,
      electronicPeriod: 42,
      electronicExampleFile: null,
      hardExtension: 1,
      hardPeriod: 42,
      hardState: { value: 'NEW', text: 'New' },
      coverPhoto: null,
      showError: false,
      errorMessage: '',
      coverPhotoPath: '',
      hardCopy: {
        maximumNumberOfExtension: undefined,
        borrowPeriod: undefined,
        state: undefined,
      },
      electronicCopy: {
        maximumNumberOfExtension: undefined,
        borrowPeriod: undefined,
        state: undefined,
      },
      options: [
        { value: 'NEW', text: 'New' },
        { value: 'USED', text: 'Used' },
        { value: 'DAMAGED', text: 'Damaged' }
      ]
    }
  },
  methods: {
    limitTextAuthor (count) {
      return `and ${count} other authors`
    },
    asyncFindAuthor(query) {
      this.isLoading = true
      let params = {params: {"name": query}};
      ApiConnect.get('/authors/', params).then(response => {
        console.log(response.data,query)
        this.authors = response.data
        this.isLoading = false
      })

    },
    clearAll () {
      this.selectedAuthors = []
    },
    getBook(id){
      ApiConnect.get('/books/'+id).then((response) =>{
        this.book = response.data
        this.book.publicationDate = new Date(this.book.publicationDate)
      });

    },
    getGenres(){
      ApiConnect.get('/genres/').then((response) =>{
        this.genres = response.data
      })
    },
    coverPhotoInputChange() {
      if (this.coverPhoto !== null){
        this.coverPhoto = file.renameFile(this.coverPhoto);
      }
      let formData = new FormData();
      formData.append('file', this.coverPhoto, this.coverPhoto.name);
      ApiConnect.post('uploadFile', formData).then((response)=> {
        let filePath = response.data.fileDownloadUri;
        this.book.coverPhotoPath = filePath;
      })
    },
    check_form(){
      let form_check_error = false;
      if (! this.book.name){
        this.$refs['title'].state = false;
        this.$refs['title'].value = "";
        form_check_error = true;
      }
      if (! this.book.language){
        this.$refs['lang'].state = false;
        this.$refs['lang'].value = "";
        form_check_error = true;
      }
      if (! this.book.publisher){
        this.$refs['publisher'].state = false;
        this.$refs['publisher'].value = "";
        form_check_error = true;
      }
      if (! this.book.isbn){
        this.$refs['isbn'].state = false;
        this.$refs['isbn'].value = "";
        form_check_error = true;
      }
      if (! this.book.publicationDate){
        this.$refs['publicationDate'].state = false;
        this.$refs['publicationDate'].value = "";
        form_check_error = true;
      }
      if (! this.book.publicationNumber){
        this.$refs['publicationNumber'].state = false;
        this.$refs['publicationNumber'].value = "";
        form_check_error = true;
      }
      if (! this.book.pages){
        this.$refs['pages'].state = false;
        this.$refs['pages'].value = "";
        form_check_error = true;
      }
      if (this.book.authors.length < 1){
        this.$refs['authors'].state = false;
        this.$refs['authors'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Authors field can not be empty.'
      }
      if (this.book.genres.length < 1 ){
        this.$refs['genres'].state = false;
        this.$refs['genres'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Genres field can not be empty.'
      }

      return form_check_error;
    },
    reset_form_state(){
      this.$refs['title'].state = null;
      this.$refs['lang'].state=null;
      this.$refs['publisher'].state=null;
      this.$refs['isbn'].state=null;
      this.$refs['publicationDate'].state=null;
      this.$refs['publicationNumber'].state=null;
      this.$refs['pages'].state=null;
      this.$refs['authors'].state=null;
      this.$refs['genres'].state=null;
    },
    submit(){
        this.reset_form_state();
        if (this.check_form()) return;
        ApiConnect.put('/books', this.book).then((response) =>{
          this.reset_form_state();
          this.makeToast('Book '+this.book.name+' has been updated successfully.')
        }).catch(error => {
          console.log(error)
        })
    },
    create(){
      this.reset_form_state();
      if (this.check_form()) return;
        ApiConnect.post('/books', this.book).then((response) =>{
          this.reset_form_state();
          this.makeToast('Book '+this.book.name+' has been created successfully.')
        }).catch(error => {
          console.log(error)
        })
      ApiConnect.get('/books/').then(resp =>{
        this.$router.push('/edit_books/'+(resp.data[resp.data.length -1].id+1))
      })
    },
    makeToast(text) {
      this.$bvToast.toast(text, {
        title: 'Library',
        variant: 'success',
        autoHideDelay: 5000,
      })
    },
    check_electronic_exemplar_form(){
      let form_check_error = false;
      if (! this.electronicExtension){
        this.$refs['maximumNumberOfExtension'].state = false;
        this.$refs['maximumNumberOfExtension'].value = "";
        form_check_error = true;
      }
      if (! this.electronicPeriod){
        this.$refs['borrowPeriod'].state = false;
        this.$refs['borrowPeriod'].value = "";
        form_check_error = true;
      }
      if (! this.electronicExampleFile){
        this.$refs['electronicExampleFileInput'].state = false;
        this.$refs['electronicExampleFileInput'].value = "";
        form_check_error = true;
      }

      return form_check_error;
    },
    addElectronicExample() {
      if (this.check_electronic_exemplar_form()) return;
      this.$refs.addElectronicCopy.hide();
      let electronicExample = {};
      electronicExample.availability = true;
      electronicExample.book = this.book;
      electronicExample.borrowPeriod = this.electronicPeriod;
      electronicExample.maximumNumberOfExtension = this.electronicExtension;
      electronicExample.state = "ELECTRONIC";
      electronicExample.titleName = this.book.name;
      electronicExample.id = 0;
      if (this.electronicExampleFile !== null){
        this.electronicExampleFile = file.renameFile(this.electronicExampleFile);
      }
      let formData = new FormData();
      formData.append('file', this.electronicExampleFile, this.electronicExampleFile.name);
      ApiConnect.post('uploadFile', formData).then((response)=> {
        let filePath = response.data.fileDownloadUri;
        electronicExample.filePath = filePath;
        ApiConnect.post('/electronic-copy-exemplars',electronicExample).then(response => {
          this.makeToast('Electronic copy was added successfully.')
          ApiConnect.get('/books/'+this.book.id).then((response) =>{
            this.book.electronicCopyExemplars = response.data.electronicCopyExemplars
          });
        })
      })
    },
    check_hard_exemplar_form(){
      let form_check_error = false;
      if (! this.hardExtension){
        this.$refs['maximumNumberOfExtension'].state = false;
        this.$refs['maximumNumberOfExtension'].value = "";
        form_check_error = true;
      }
      if (! this.hardPeriod){
        this.$refs['borrowPeriod'].state = false;
        this.$refs['borrowPeriod'].value = "";
        form_check_error = true;
      }

      return form_check_error;
    },
    addHardExample() {
      if (this.check_hard_exemplar_form()) return;
      this.$refs.addHardCopy.hide();
      let hardExample = {};
      hardExample.availability = true;
      hardExample.book = this.book;
      hardExample.borrowPeriod = this.hardPeriod;
      hardExample.maximumNumberOfExtension = this.hardExtension;
      hardExample.state = this.hardState.value;
      hardExample.titleName = this.book.name;
      hardExample.id = 0;
      ApiConnect.post('/hard-copy-exemplars',hardExample).then(response => {
        this.makeToast('Hard copy was added successfully.')
        ApiConnect.get('/books/'+this.book.id).then((response) =>{
          this.book.hardCopyExemplars = response.data.hardCopyExemplars
        });
      })
    },
    editHardExample(){
      this.hardCopy.state = this.hardCopy.state.value;
      ApiConnect.put('/hard-copy-exemplars',this.hardCopy).then(response => {
        this.makeToast('Hard copy was updated successfully.')
      })
      this.$refs.updateHardCopy.hide();
    },
    editElectronicExample(){
      ApiConnect.put('/electronic-copy-exemplars',this.electronicCopy).then(response => {
        this.makeToast('Electronic copy was updated successfully.')
      })
      this.$refs.updateElectronicCopy.hide();
    },
    deleteHardCopy(id){
      ApiConnect.delete('/hard-copy-exemplars/'+id).then(response => {
        this.makeToast('Hard copy exemplar was successfully deleted')
        ApiConnect.get('/books/'+this.book.id).then((response) =>{
          this.book.hardCopyExemplars = response.data.hardCopyExemplars
        });
      });
    },
    deleteElectroCopy(id){
      ApiConnect.delete('/electronic-copy-exemplars/'+id).then(response => {
        this.makeToast('Electronic copy exemplar was successfully deleted')
        ApiConnect.get('/books/'+this.book.id).then((response) =>{
          this.book.electronicCopyExemplars = response.data.electronicCopyExemplars
        });
      });
    },
    editHardCopy(hardCopy){
      this.hardCopy = hardCopy;
      if (this.hardCopy.state == 'NEW') this.hardCopy.state ={ value: 'NEW', text: 'New' };
      if (this.hardCopy.state == 'DAMAGED') this.hardCopy.state ={ value: 'DAMAGED', text: 'Damaged' };
      if (this.hardCopy.state == 'USED') this.hardCopy.state ={ value: 'USED', text: 'Used' };
      this.$refs.updateHardCopy.show();
    },
    editElectronicCopy(electronicCopy){
      console.log(electronicCopy)
      this.electronicCopy = electronicCopy;
      this.$refs.updateElectronicCopy.show();
    }
  },
  created() {
    if(this.$route.params.id == 0){
      this.book = {
            id: 0,
            name: '',
            description: '',
            publisher: '',
            language: '',
            coverPhoto: '',
            authors: [],
            hardCopyExemplars: [],
            electronicCopyExemplars: [],
            isbn: undefined,
            publicationNumber: 1,
            publicationDate: new Date(),
            pages: '',
            genres: [],
      }
    }else{
      this.getBook(this.$route.params.id);
    }
    this.getGenres();
  }
}
</script>
<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
<style scoped>
.edit-book-page{
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
.del-btn:hover{
  background-color: #bac1c0;
}
</style>