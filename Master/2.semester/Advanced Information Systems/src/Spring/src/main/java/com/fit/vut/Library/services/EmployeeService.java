package com.fit.vut.Library.services;

import com.fit.vut.Library.application.PasswordEncryptor;
import com.fit.vut.Library.dtos.*;
import com.fit.vut.Library.entities.Employee;
import com.fit.vut.Library.repositories.EmployeeRepository;
import com.fit.vut.Library.services.filters.UserFilter;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.persistence.criteria.*;
import java.util.*;

@Service
@Transactional
public class EmployeeService {

    @PersistenceContext
    private EntityManager em;
    private final EmployeeRepository repo;

    public EmployeeService(EmployeeRepository repo) {
        this.repo = repo;
    }

    public void create (FullUserDto u) throws Exception {
        if ( ! repo.findByEmail(u.getEmail()).isEmpty()) {
            throw new Exception();
        }
        Employee employee = new Employee(0L, u.getEmail(), PasswordEncryptor.encrypt(u.getPassword()), u.getName(),
                u.getSurname(), u.getStreet(), u.getHouseNumber(), u.getCity(), u.getPostcode(), u.getRole());
        repo.save(employee);
    }

    public void update (EmployeeDto e){
        Employee employee = repo.getById(e.getId());
        employee.setName(e.getName());
        employee.setSurname(e.getSurname());
        employee.setEmail(e.getEmail());
        employee.setStreet(e.getStreet());
        employee.setHouseNumber(e.getHouseNumber());
        employee.setCity(e.getCity());
        employee.setPostcode(e.getPostcode());
        repo.save(employee);
    }

    public void updatePassword(NewPasswordDto metadata){
        Employee employee = repo.getById(metadata.getUserId());
        String newPassword = PasswordEncryptor.encrypt(metadata.getPassword());
        employee.setHashPassword(newPassword);
        repo.save(employee);
    }

    public void delete (Long id){
        repo.delete(new Employee(id));
    }

    public EmployeeDto getById (Long id){
        Optional<Employee> employee = repo.findById(id);
        return employee.isEmpty() ? null : employee.get().toDto();
    }

    public EmployeeDto getByCredentials (CredentialsDto credentials){
        List<Employee> employees = repo.findByEmail(credentials.getEmail());
        if (employees.isEmpty())
            return null;

        Employee employee = employees.stream().findFirst().get();
        return PasswordEncryptor.verify(credentials.getPassword(), employee.getHashPassword()) ? employee.toDto() : null;
    }

    public List<EmployeeDto> get(UserFilter filter) {
        if (filter == null)
            repo.findAll().stream().map(x -> x.toDto()).toList();

        CriteriaBuilder cb = em.getCriteriaBuilder();
        CriteriaQuery<Employee> cq = cb.createQuery(Employee.class);
        Root<Employee> quest = cq.from(Employee.class);
        List<Predicate> predicates = new ArrayList();

        if (filter.getName() != null)
            predicates.add(cb.like(quest.get("name"), "%" + filter.getName() + "%"));

        if (filter.getSurname() != null)
            predicates.add(cb.like(quest.get("surname"), "%" + filter.getSurname() + "%"));

        if (filter.getEmail() != null)
            predicates.add(cb.like(quest.get("email"), "%" + filter.getEmail() + "%"));

        if (filter.getStreet() != null)
            predicates.add(cb.like(quest.get("street"), "%" + filter.getStreet() + "%"));

        if (filter.getHouseNumber() != null)
            predicates.add(cb.like(quest.get("houseNumber"), "%" + filter.getHouseNumber() + "%"));

        if (filter.getCity() != null)
            predicates.add(cb.like(quest.get("city"), "%" + filter.getCity() + "%"));

        if (filter.getPostcode() != null)
            predicates.add(cb.like(quest.get("postcode"), "%" + filter.getPostcode() + "%"));

        if (filter.getRole() != null)
            predicates.add(cb.like(quest.get("role"), "%" + filter.getRole() + "%"));

        cq.select(quest).where(predicates.toArray(new Predicate[] {}));
        List<Employee> employees = em.createQuery(cq).getResultList();
        return employees.stream().map(x -> x.toDto()).toList();
    }
}
