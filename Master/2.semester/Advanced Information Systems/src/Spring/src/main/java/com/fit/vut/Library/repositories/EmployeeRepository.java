package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.Employee;
import com.fit.vut.Library.entities.Genre;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface EmployeeRepository extends JpaRepository<Employee, Long> {
    @Query(value = "SELECT e from Employee e WHERE e.email = :email")
    List<Employee> findByEmail(@Param("email") String email);
}
