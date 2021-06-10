"use strict";

class Employee {
    constructor() {
        if (this.constructor === Employee) {
            throw new Error("Abstract classes can't be instantiated.");
        }
    }
    
    getAverageMonthlySalary() {
        throw new Error("Abstract method doesn't have an implementation.")
    }
}

class FixedSalaryEmployee extends Employee {
    constructor({id, name, salary}) {
        super();
        this.id  = id;
        this.name = name;
        this.salary = salary;
    }

    id() {return this.id}
    name() {return this.name}
    salary() {return this.salary}
    
    getAverageMonthlySalary() {
        return this.salary;
    }
}

class PerHourSalaryEmployee extends Employee {
    constructor({id, name, salary}) {
        super();
        this.id  = id;
        this.name = name;
        this.salary = salary;
    }

    id() {return this.id}
    name() {return this.name}
    salary() {return this.salary}
    
    getAverageMonthlySalary() {
        return 20.8 * 8 * this.salary;
    }
}

class Workers {
    constructor(workers) {
        this.workers = workers;
    }

    getWorkers() {return this.workers.map(function (worker) {return {id: worker.id, name: worker.name, averageSalary: worker.getAverageMonthlySalary()}})}

    getFirstEmployeeNames(amount) {
        return this.workers.slice(0, amount).map(worker => worker.name);
    }

    getLastEmployeeIds(amount) {
        return this.workers.slice(this.workers.length - amount, this.workers.length).map(worker => worker.id);
    }

    sortByAverageMonthlySalaryDesc() {
        this.workers = this.workers.sort(function(first, second) {
            const firstSalary = first.getAverageMonthlySalary();
            const secondSalary = second.getAverageMonthlySalary();
            if (firstSalary < secondSalary) {
                return 1;
            }
            else if (firstSalary > secondSalary) {
                return -1;
            }
            else{
                if (first.name < second.name) {
                    return 1
                }
                else if(first.name > second.name){
                    return -1;
                }
            }
            return 0;
        });
    }
}

let workers = new Workers([
    new FixedSalaryEmployee({id: 'id0', name: '0', salary: 0}),
    new PerHourSalaryEmployee({id: 'id3', name: '3', salary: 3}),
    new PerHourSalaryEmployee({id: 'id4', name: '4', salary: 4}),
    new FixedSalaryEmployee({id: 'id1', name: '1', salary: 1}),
    new FixedSalaryEmployee({id: 'id2', name: '2', salary: 2}),
    new PerHourSalaryEmployee({id: 'id5', name: '5', salary: 5}),
])

function countInputChange(value) {
    const element = document.getElementById("count-input");
    const newValue = +element.value + +value
    if (newValue >= 0){
        element.value = newValue;
    }
}

function sortWorkers() {
    workers.sortByAverageMonthlySalaryDesc()
    rerenderTheTable(workers.getWorkers());
}

function getFirstNames() {
    let amount = +document.getElementById("count-input").value;
    if(amount === 0) {
        amount = 5;
    }
    document.getElementById("get-result-list").innerHTML =
        workers.getFirstEmployeeNames(amount).map(name => `<li><span>${name}</span></li>`).join('');
}

function getLastIds() {
    let amount = +document.getElementById("count-input").value;
    if(amount === 0) {
        amount = 3;
    }
    document.getElementById("get-result-list").innerHTML =
        workers.getLastEmployeeIds(amount).map(id => `<li><span>${id}</span></li>`).join('');
}


function rerenderTheTable(workers) {
    document.getElementById("employee-table-body").innerHTML =
        workers.map(worker =>
            `<tr><td>${worker.id}</td><td>${worker.name}</td><td>${worker.averageSalary}</td></tr>`
        ).join('');
}

rerenderTheTable(workers.getWorkers());

function loadFromTextArea() {
    const value = document.getElementById("json-input").value;
    if(!value) {
        document.getElementById("error-message").style.display = "block";
        return;
    }
    workers = new Workers(mapFromJson(JSON.parse(value)));
    rerenderTheTable(workers.getWorkers());
}

function closeErrorMessage() {
    document.getElementById("error-message").style.display = "none";
}

function mapFromJson(json) {
    const workers = [];
    json.forEach(j => {
        let worker = {};
        switch(j.type){
            case "FixedSalaryEmployee":
                worker = new FixedSalaryEmployee({id: j.id, name: j.name, salary: j.salary});
                break;
            case "PerHourSalaryEmployee":
                worker = new PerHourSalaryEmployee({id: j.id, name: j.name, salary: j.salary});
                break;
        }
        workers.push(worker);
    })
    return workers;
}

function loadData() {
    const element = document.getElementById("switch");
    if(element.checked) {
        loadDataFromFile();
    } else {
        loadFromTextArea();
    }
}

function loadDataFromFile() {
    const fileLoaderId = "loader";
    if (!document.getElementById(fileLoaderId)) {
        const fileInput = document.createElement("input");
        fileInput.type = "file";
        fileInput.style.display = "none";
        fileInput.id = fileLoaderId;
        fileInput.accept = ".json";
        fileInput.onchange = function (file) {
            const selectedFile = document.getElementById(fileLoaderId).files[0];
            if (selectedFile) {
                const reader = new FileReader();
                reader.readAsText(selectedFile, "UTF-8");
                reader.onload = function (text) {
                    const result = text.target.result;
                    document.getElementById("json-input").innerHTML = result;
                    workers = new Workers(mapFromJson(JSON.parse(result)));
                    rerenderTheTable(workers.getWorkers());
                };
            }

            document.getElementById(fileLoaderId)?.remove();
        };

        document.body.appendChild(fileInput);
    }
    document.getElementById(fileLoaderId).click();
}