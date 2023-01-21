package com.fit.vut.Library.repositories;

import com.fit.vut.Library.entities.Field;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;
import java.util.List;

@Repository
public interface FieldRepository extends JpaRepository<Field, Long> {

    @Query(value = "SELECT f from Field f WHERE f.name = :name")
    List<Field> findByName(@Param("name") String name);
}
