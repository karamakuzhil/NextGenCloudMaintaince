using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Device.Persistance.EntityConfigurations
{
    class DeviceEntityConfiguration : IEntityTypeConfiguration<Devices.Domain.AggregatesModel.Device>
    {
        public void Configure(EntityTypeBuilder<Devices.Domain.AggregatesModel.Device> deviceConfiguration)
        {
            deviceConfiguration.ToTable("devices", DeviceDbContext.DEFAULT_SCHEMA);

            deviceConfiguration.HasKey(o => o.DeviceId);
        }
    }
}
