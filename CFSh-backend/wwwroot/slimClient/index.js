const app = Vue.createApp({})

const appheaderComp = {
  data() {
    return {
      filter_base: ""
    }
  },
  props: ['username', 'is-upload'],
  emits: ['log-out', 'upload-file', 'back-to-list', 'filter-change'],
  methods: {
    isUploadTrue() {
      return this.isUpload
    }
  },
  computed: {
    filter: {
      get() {
        return this.filter_base
      },
      set(value) {
        this.filter_base = value
        this.$emit('filter-change', value)
      }
    }
  },
  template: `<table class="menu">
              <tr>
                <td v-if='isUploadTrue()' class="menuelem"><button class="menubutton" @click="$emit('back-to-list')">Back</button></td>
                <td v-else class="menuelem"><button class="menubutton" @click="$emit('upload-file')">Upload</button></td>
                <td class="filter-menu-elem"><input v-bind:disabled="isUploadTrue()" type=text class="file-filter" placeholder="Filter file name..." v-model="filter"/></td>
                <td class="menuelem"><button class="menubutton" @click="$emit('log-out')">Logout</button></td>
              </tr>
            </table>`
}

const uploadComp = {
  props: ['userid'],
  data() {
    return {
      prog: 0,
      status: "File not selected...",
      filenameke: ""
    }
  },
  methods: {
    onChange(e) {
      const file = document.getElementById('up-file-input').files[0];
      if (file) {
        this.filenameke = file.name
        this.processFile(file);
      }
    },
    processFile(file) {
      const fr = new FileReader();
      fr.readAsBinaryString(file);
      fr.onerror = this.errorHandler;
      fr.onabort = () => this.changeStatus('Start Loading');
      fr.onloadstart = () => this.changeStatus('Start Loading');
      fr.onload = () => {
        this.changeStatus('Loaded')
      };
      fr.onloadend = this.loaded;
      fr.onprogress = this.setProgress;
    },
    setProgress(e) {
      console.log("In setProgress");
      const fr = e.target;
      const loadingPercentage = 100 * e.loaded / e.total;
      this.prog = loadingPercentage;
    },
    changeStatus(status) {
      this.status = status
    },
    loaded(e) {
      this.changeStatus('Load ended!');
      console.log(e)
      this.prog = 100;
      const fr = e.target
      var result = fr.result;

      console.log('result filename:')
      console.log(result)
      this.upload(result, e.target.FileName)
      console.info("TODO implement upload to server")
    },
    errorHandler(e) {
      this.changeStatus("Error: " + e.target.error.name)
    },
    upload(content, filename) {
      const Http = new XMLHttpRequest();
      const url='http://127.0.0.1:5000/api/File/';
      const reqDat = {
        "Content": btoa(content),
        "Name": this.filenameke,
        "Owner" : this.userid
      }

      //xhr.setRequestHeader('Authorization', 'Bearer ' + access_token);
      //xhr.onload = requestComplete;

      Http.open("POST", url);
      Http.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
      Http.onreadystatechange = (e) => {
        console.log(Http.statusText)
        this.onAnswer(Http.responseText)
      }
      Http.send(JSON.stringify(reqDat));

      
    },
    onAnswer(response) {
      console.log("Upload resp " + response)
      if(response !== "") {
      }
    }
  },
  template: `<div class="container">
             <table class="table-center">
              <tr>
                <td class="td-center">
                    <div class="center">
                      <div class="progress-cont"><div style="display: inline-block; width: 20%;">{{ status }}</div><progress style="width: 80%;" :value="prog" max="100"></progress></div>
                      <input style="width: 100%;" class="gen-button" id="up-file-input" type="file" @change="onChange($event)"/>
                    </div>
                </td>
              </tr>
            </table>
            </div>`
}

const loginComp = {
  data() {
    return {
      username: ""
    }
  },
  methods: {
    login() {
      console.info("TODO implement login")
      const Http = new XMLHttpRequest();
      const url='http://localhost:5000/api/Users/authenticate';
      const reqDat = {
        "username": "admin",
        "password": "pass"
      }

      //xhr.setRequestHeader('Authorization', 'Bearer ' + access_token);
      //xhr.onload = requestComplete;

      Http.open("POST", url);
      Http.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
      Http.onreadystatechange = (e) => {
        console.log("Login stat: " + Http.statusText)
        this.onAnswer(Http.responseText)
      }
      Http.send(JSON.stringify(reqDat));

      
    },
    onAnswer(response) {
      console.log("Login resp" + response)
      if(response !== "") {
        this.$emit('logged-in', response.Id)
      }
    }
  },
  emits: ['logged-in', 'register-req'],
  template: `<table class="container-little">
              <tr>
                <td class="td-center">
                  <form class="center-little" v-on:submit.prevent="login()">
                    <table>
                      <tr>
                        <td><label class="formelem" for="username">Username:</label></td>
                        <td><input class="formelem" type="text" id="username" name="username" v-model=username></td>
                      </tr>
                      <tr>
                        <td><label class="formelem" for="password">Password:</label></td>
                        <td><input class="formelem" type="password" id="password" name="password"></td>
                      </tr>
                      <tr>
                        <td class="td-center" colspan="2"><input class="gen-button" type="submit" value='Login'></td>
                      </tr>
                      <tr>
                        <td class="td-center" colspan="2">
                          <label class="smallfont">Not signed up?</label>
                          <button class="gen-button" @click="$emit('register-req')">Register</button>
                        </td>
                      </tr>
                    </table>
                  </form>
                </td>
              </tr>
            </table>`
}

const registerComp = {
  data() {
    return {
        username: "",
        password1: "",
        password2: "",
        firstname: "",
        lastname: "",
        isError: false,
        errormsg: ""
    }
  },
  methods: {
    register() {
        if(this.password1 !== this.password2) {
            this.isError = true
            this.errormsg = "The passwords must be the same!"
            return
        }
        else {
            this.isError = false
            this.errormsg = ""
        }
      console.info("TODO implement register")
      const Http = new XMLHttpRequest();
      const url='site:/api/Users/register';
      const reqDat = {
        "username": this.username,
        "Password": this.password1,
        "FirstName": this.firstname,
        "LastName": this.lastname
      }

      //xhr.setRequestHeader('Authorization', 'Bearer ' + access_token);
      //xhr.onload = requestComplete;

      Http.open("POST", url);
      Http.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
      Http.onreadystatechange = (e) => {
        console.log(Http.statusText)
        this.onAnswer(Http.responseText)
      }
      Http.send(JSON.stringify(reqDat));

      
    },
    onAnswer(response) {
      console.log(response)
      if(response !== "") {
        this.$emit('logged-in', this.username)
      }
    }
  },
  emits: ['logged-in', 'login-req'],
  template: `<table class="container-little">
              <tr>
                <td class="td-center">
                  <form class="center-little" v-on:submit.prevent="register()">
                    <table>
                      <tr>
                        <td><label class="formelem" for="firstnameid">*First Name:</label></td>
                        <td><input class="formelem" type="text" id="firstnameid" name="firstnameid" v-model=firstname></td>
                      </tr>
                      <tr>
                        <td><label class="formelem" for="lastnameid">*Last Name:</label></td>
                        <td><input class="formelem" type="text" id="lastnameid" name="lastnameid" v-model=lastname></td>
                      </tr>
                      <tr>
                        <td><label class="formelem" for="usernameid">*Username:</label></td>
                        <td><input class="formelem" type="text" id="usernameid" name="usernameid" v-model=username></td>
                      </tr>
                      <tr>
                        <td><label class="formelem" for="password">*Password:</label></td>
                        <td><input class="formelem" type="password" id="password" name="password" v-model=password1></td>
                      </tr>
                      <tr>
                        <td><label class="formelem" for="rpassword">*Retype password:</label></td>
                        <td><input class="formelem" type="password" id="rpassword" name="rpassword" v-model=password2></td>
                      </tr>
                      <tr v-if="isError">
                        <td><label style="color: red;" class="formelem">{{errormsg}}</label></td>
                      </tr>
                      <tr>
                        <td><button class="gen-button" @click="$emit('login-req')">Login</button></td>
                        <td class="td-center" colspan="2"><input class="gen-button" type="submit" value='Register'></td>
                      </tr>
                    </table>
                  </form>
                </td>
              </tr>
            </table>`
}

const fileListComp = {
  props: ['files', 'filter'],
  methods: {
    filterName(fileName) {
      return fileName.includes(this.filter)
    },
    download(id, name) {
      console.info("TODO implement files")
      const Http = new XMLHttpRequest();
      const url='http://127.0.0.1:5000/api/File/' + id;

      //xhr.setRequestHeader('Authorization', 'Bearer ' + access_token);
      //xhr.onload = requestComplete;

      Http.open("GET", url);
      Http.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
      Http.onreadystatechange = (e) => {
        if(Http.readyState === 4) {
          console.log("Donwload" + Http.statusText)
          this.onAnswer(Http.response, name)
        }
      }
      Http.send();

      
    },
    onAnswer(response, name) {
      
        console.log("download" + response)
        var jsonObj = JSON.parse(response);
        console.log("download OBJ")
        console.log(jsonObj)
        if(jsonObj) {
          this.downloadFile(name, atob(jsonObj.content))
        }
    },
    downloadFile(filename, text) {
      var element = document.createElement('a');
      element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(text));
      element.setAttribute('download', filename);
    
      element.style.display = 'none';
      document.body.appendChild(element);
    
      element.click();
    
      document.body.removeChild(element);
    }
  },
  template: `<div class="container">
              <table class="table-center">
              <tr>
                <td class="td-center">
                    <div class="center">
                      <div v-if="files.length === 0">
                          <label class="formelem">You don't have any files yet!</label>
                      </div>
                      <template v-for="file in files">
                        <div v-if="filterName(file.name)" class="file-cont">
                          <button style="width: 80%" class="file-button" @click="download(file.id, file.name)">{{ file.name }}</button>
                          <button style="width: 20%" class="file-button" disabled=true>Delete</button>
                        </div>
                      </template>
                    </div>
                </td>
              </tr>
              </table>
            </div>`
}

app.component('root-comp', {
  data() {
    return {
      greetname: "",
      currentView: 'Login',
      filter: "",
      isMenu: false,
      views: ['Login', 'Register', 'List', 'Upload'],
      files: []
    }
  },
  methods: {
    isLogin() {
      return this.currentView === 'Login'
    },
    isRegister() {
      return this.currentView === 'Register'
    },
    isList() {
      return this.currentView === 'List'
    },
    isHeader() {
      return this.isMenu
    },
    isUpload() {
      return this.currentView === 'Upload'
    },
    onLoggedIn(username) {
      this.greetname = username
      this.currentView = 'List'
      this.isMenu = true
      this.getFiles()
    },
    showList() {
      this.currentView = 'List'
      this.getFiles()
    },
    onLogin() {
      this.currentView = 'Login'
    },
    onUploadFile() {
      this.currentView = 'Upload'
    },
    onRegister() {
      this.currentView = 'Register'
    },
    onLogout() {
      this.currentView = 'Login'
      this.isMenu = false
    },
    getFiles() {
      console.info("TODO implement getfiles")
      const Http = new XMLHttpRequest();
      const url='http://127.0.0.1:5000/api/File/user/' + this.greetname;

      //xhr.setRequestHeader('Authorization', 'Bearer ' + access_token);
      //xhr.onload = requestComplete;

      Http.open("GET", url);
      Http.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
      Http.onreadystatechange = (e) => {
        if(Http.readyState === 4) {
        console.log("GetFiles" + Http.statusText)
        this.onAnswer(Http.response)
        }
      }
      Http.send();

      
    },
    onAnswer(response) {
      
        console.log("GetFiles" + response)
        var jsonObj = JSON.parse(response);
        console.log("JSON OBJ")
        console.log(jsonObj)
        this.files = jsonObj
        var asd = []
        if(jsonObj) {
          for(var i in jsonObj) {
            if(i !== undefined)
              asd.push({name: jsonObj[i].name, id: jsonObj[i].id});
          }
          this.files = asd
        }
    }
  },
  components: {
    'appheader': appheaderComp,
    'login': loginComp,
    'register': registerComp,
    'file-list': fileListComp,
    'upload': uploadComp
  },
  template: `<appheader v-if="isHeader()" @filter-change="filter=$event" :is-upload="isUpload()" @back-to-list="showList()" @log-out="onLogout()" @upload-file="onUploadFile()"></appheader>
    <login v-if="isLogin()" @logged-in="onLoggedIn($event)" @register-req="onRegister()"></login>
    <register v-if="isRegister()" @logged-in="onLoggedIn($event)" @login-req="onLogin()"></register>
    <file-list v-if="isList()" :filter="filter" :files=files></file-list>
    <upload v-if="isUpload()" :userid="greetname"></upload>`
})

var vs = app.mount('#mainapp')
