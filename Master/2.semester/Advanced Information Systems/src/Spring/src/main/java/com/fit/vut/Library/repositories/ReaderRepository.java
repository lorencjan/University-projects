package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.Employee;
import com.fit.vut.Library.entities.Reader;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ReaderRepository extends JpaRepository <Reader, Long>{
    @Query(value = "SELECT r from Reader r WHERE r.email = :email")
    List<Reader> findByEmail(@Param("email") String email);
}

