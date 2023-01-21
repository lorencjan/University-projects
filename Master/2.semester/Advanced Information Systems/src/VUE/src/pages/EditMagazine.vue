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
          preview magazine page
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
                  v-model="magazine.name"
                  type="text"
                  placeholder="Enter magazine title"
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
                  v-model="magazine.language"
                  type="text"
                  placeholder="Enter magazine Language"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Language field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
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
                  v-model="magazine.publisher"
                  type="text"
                  placeholder="Enter magazine publisher"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Publisher field can not be empty
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <b-form-group
                id="issn-label"
                label="ISSN:"
                label-class="required"
                label-for="issn"
            >
              <b-form-input
                  ref="issn"
                  id="issn"
                  v-model="magazine.issn"
                  type="number"
                  placeholder="Enter magazine issn"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                ISSN field can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="number-label"
                label="Magazine number:"
                label-class="required"
                label-for="number"
            >
              <b-form-input
                  ref="number"
                  id="number"
                  v-model="magazine.number"
                  type="number"
                  placeholder="Enter magazine number"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Magazine number can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
          <b-col>
            <b-form-group
                id="publicationYear-label"
                label="Magazine publication year:"
                label-class="required"
                label-for="publicationYear"
            >
              <b-form-input
                  ref="publicationYear"
                  id="publicationYear"
                  v-model="magazine.publicationYear"
                  type="number"
                  placeholder="Enter magazine publication number"
                  required
              ></b-form-input>
              <b-form-invalid-feedback>
                Magazine publication year can not be empty.
              </b-form-invalid-feedback>
            </b-form-group>
          </b-col>
        </b-row>
        <b-row>
          <b-col>
            <label class="typo__label" for="authors">Authors</label>
            <multiselect
                v-model="magazine.authors"
                id="authors"
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
                <div class="multiselect__clear" v-if="magazine.authors.length" @mousedown.prevent.stop="clearAll(props.search)"></div>
              </template>
              <template slot="noResult">
                <span>Oops! No authors found. Consider changing the search query.</span>
              </template>
            </multiselect>
          </b-col>
          <b-col>
            <label class="typo__label">Fields </label>
            <multiselect
                v-model="magazine.fields"
                :options="fields"
                :multiple="true"
                :close-on-select="false"
                :clear-on-select="false"
                :preserve-search="true"
                placeholder="Pick some"
                label="name"
                track-by="name"
                :preselect-first="true">
              <template slot="clear" slot-scope="props">
                <div class="multiselect__clear" v-if="magazine.fields.length" @mousedown.prevent.stop="clearAll(props.search)"></div>
              </template>
              <template slot="noResult">
                <span>Oops! No fields found. Consider changing the search query.</span>
              </template>
            </multiselect>
          </b-col>
        </b-row>
        <b-row class="mt-3">
          <b-col cols="6" class="text-left "><label>Cover photo</label><br>
            <div style="position: initial">
              <b-form-file
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
              <template v-for="hardCopy in magazine.hardCopyExemplars">
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
              <template v-for="Copy in magazine.electronicCopyExemplars">
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
                v-model="magazine.description"
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
              :img="magazine.coverPhoto"
              format="Magazine"
              :publisher="magazine.publisher"
              :released="magazine.publicationDate"
              :pages="magazine.pages"
              :hasElectronicCopy="hasElectronicCopy"
              :hardCopies="magazine.hardCopyExemplars"
              :electronicCopies="magazine.electronicCopyExemplars"
          >

          </BookTitle>
        </b-col>
        <b-col cols="8">
          <BookInfo
              :title="magazine.name"
              :publicationNumber="magazine.number + '/' + magazine.pubcationYear"
              :authors="magazine.authors"
              :issn="magazine.issn"
              :genres="magazine.fields"
              :description="magazine.description"
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
            Number of extensions can not be empty.
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
            Electronic example file can not be empty.
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
        </b-form-group>
        <b-form-group
            id="state-label"
            label="Books state:"
            label-class="required"
            label-for="state"
        >
          <multiselect
              ref="state"
              id="state"
              v-model="hardState"
              :options ="options"
              label="text"
              track-by="text"
              type="text"
              placeholder="Select exemplar state"
              required
          ></multiselect>
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
              type="text"
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
import * as file from "@/assets/js/file";
import NavbarFinal from "@/components/main_page/NavbarFinal";

export default {
  name: "EditBook",
  components: {
    BookInfo,
    BookTitle,
    Multiselect,
    NavbarFinal
  },
  data () {
    return {
      authors: [],
      isLoading: false,
      fields: [],
      isLoadingGenre: false,
      magazine: {},
      electronicExtension: 1,
      electronicPeriod: 42,
      hardExtension: 1,
      hardPeriod: 42,
      hardState: { value: 'NEW', text: 'New' },
      coverPhoto: null,
      electronicExampleFile: null,
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
      ],
      showError: false,
      errorMessage: '',
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
    getMagazine(id){
      ApiConnect.get('/magazines/'+id).then((response) =>{
        this.magazine = response.data
      });
    },
    getFields(){
      ApiConnect.get('/fields/').then((response) =>{
        this.fields = response.data
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
        this.magazine.coverPhotoPath = filePath;
      })
    },
    check_form(){
      let form_check_error = false;
      if (! this.magazine.name){
        this.$refs['title'].state = false;
        this.$refs['title'].value = "";
        form_check_error = true;
      }
      if (! this.magazine.language){
        this.$refs['lang'].state = false;
        this.$refs['lang'].value = "";
        form_check_error = true;
      }
      if (! this.magazine.publisher){
        this.$refs['publisher'].state = false;
        this.$refs['publisher'].value = "";
        form_check_error = true;
      }
      if (! this.magazine.issn){
        this.$refs['issn'].state = false;
        this.$refs['issn'].value = "";
        form_check_error = true;
      }
      if (! this.magazine.number){
        this.$refs['number'].state = false;
        this.$refs['number'].value = "";
        form_check_error = true;
      }
      if (! this.magazine.publicationYear){
        this.$refs['publicationYear'].state = false;
        this.$refs['publicationYear'].value = "";
        form_check_error = true;
      }
      if (this.magazine.authors.length < 1){
        this.$refs['authors'].state = false;
        this.$refs['authors'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Authors field can not be empty.'
      }
      if (this.magazine.fields.length < 1 ){
        this.$refs['fields'].state = false;
        this.$refs['fields'].value = "";
        form_check_error = true;
        this.showError = true;
        this.errorMessage = 'Fields field can not be empty.'
      }
      return form_check_error;
    },
    reset_form_state() {
      this.$refs['title'].state=null;
      this.$refs['lang'].state=null;
      this.$refs['publisher'].state=null;
      this.$refs['issn'].state=null;
      this.$refs['publicationYear'].state=null;
      this.showError=false;
    },
    submit(){
      this.reset_form_state();
      if (this.check_form()) return;
      ApiConnect.put('/magazines', this.magazine).then((response) =>{
        this.reset_form_state();
        this.makeToast('magazine '+this.magazine.name+' has been updated successfully.')
      }).catch(error => {
        console.log(error)
      })
    },
    create(){
      this.reset_form_state();
      if (this.check_form()) return;
      console.log(this.magazine);
      ApiConnect.post('/magazines', this.magazine).then((response) =>{
        console.log(response)
        this.reset_form_state();
        this.makeToast('Magazine '+this.magazine.name+' has been created successfully.')
        ApiConnect.get('/magazines/').then(resp =>{
          this.$router.push('/edit_magazines/'+(resp.data[resp.data.length -1].id+1))
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
      electronicExample.magazine = this.magazine;
      electronicExample.borrowPeriod = this.electronicPeriod;
      electronicExample.maximumNumberOfExtension = this.electronicExtension;
      electronicExample.state = "ELECTRONIC";
      electronicExample.titleName = this.magazine.name;
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
          ApiConnect.get('/magazines/'+this.magazine.id).then((response) =>{
            this.magazine.electronicCopyExemplars = response.data.electronicCopyExemplars
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
      hardExample.magazine = this.magazine;
      hardExample.borrowPeriod = this.hardPeriod;
      hardExample.maximumNumberOfExtension = this.hardExtension;
      hardExample.state = this.hardState.value;
      hardExample.titleName = this.magazine.name;
      hardExample.id = 0;
      ApiConnect.post('/hard-copy-exemplars',hardExample).then(response => {
        this.makeToast('Hard copy was added successfully.')
        ApiConnect.get('/magazines/'+this.magazine.id).then((response) =>{
          this.magazine.hardCopyExemplars = response.data.hardCopyExemplars
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
        ApiConnect.get('/magazines/'+this.magazine.id).then((response) =>{
          this.magazine.hardCopyExemplars = response.data.hardCopyExemplars
        });
      });
    },
    deleteElectroCopy(id){
      ApiConnect.delete('/electronic-copy-exemplars/'+id).then(response => {
        this.makeToast('Electronic copy exemplar was successfully deleted')
        ApiConnect.get('/magazines/'+this.magazine.id).then((response) =>{
          this.magazine.electronicCopyExemplars = response.data.electronicCopyExemplars
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
      this.magazine = {
        id: 0,
        name: '',
        description: '',
        publisher: '',
        language: '',
        coverPhoto: '',
        authors: [],
        hardCopyExemplars: [],
        electronicCopyExemplars: [],
        issn: undefined,
        number: undefined,
        publicationYear: undefined,
        fields: [],
      }
    }else{
      this.getMagazine(this.$route.params.id);
    }
    this.getFields();
  },
  computed: {
    hasElectronicCopy (){
      if (this.magazine !== 'undefined'){
        if (this.magazine.electronicCopyExemplars.length > 0	){
          return true;
        }
      }

      return false;
    },
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
</style>