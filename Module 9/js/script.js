"use strict";

class Employee {
    constructor({id, name, salary}) {
        this.id  = id;
        this.name = name;
        this.salary = salary;
    }
    
    id() {return this.id}
    name() {return this.name}
    salary() {return this.salary}
    
    getAverageMonthlySalary() {}
}

class FixedSalaryEmployee extends Employee {
    getAverageMonthlySalary() {
        return this.salary;
    }
}

class PerHourSalaryEmployee extends Employee {
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
    const amount = +document.getElementById("count-input").value;
    document.getElementById("get-result-list").innerHTML =
        workers.getFirstEmployeeNames(amount).map(name => `<li><span>${name}</span></li>`).join('');
}

function getLastIds() {
    const amount = +document.getElementById("count-input").value;
    document.getElementById("get-result-list").innerHTML =
        workers.getLastEmployeeIds(amount).map(id => `<li><span>${id}</span></li>`).join('');
}


function rerenderTheTable(workers) {
    document.getElementById("employee-table-body").innerHTML =
        workers.map(worker =>
            `<tr><td>${worker.id}</td><td>${worker.name}</td><td>${worker.averageSalary}</td></tr>`
        ).join('');
}

function loadFromTextArea() {
    const element = document.getElementById("json-input");
    workers = new Workers(mapFromJson(JSON.parse(element.value)));
    rerenderTheTable(workers.getWorkers());
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

let workers = new Workers([
    new FixedSalaryEmployee({id: 'id0', name: '0', salary: 0}),
    new PerHourSalaryEmployee({id: 'id3', name: '3', salary: 3}),
    new PerHourSalaryEmployee({id: 'id4', name: '4', salary: 4}),
    new FixedSalaryEmployee({id: 'id1', name: '1', salary: 1}),
    new FixedSalaryEmployee({id: 'id2', name: '2', salary: 2}),
    new PerHourSalaryEmployee({id: 'id5', name: '5', salary: 5}),
])

rerenderTheTable(workers.getWorkers());
