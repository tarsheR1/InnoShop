using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.Application.Interfaces
{
    public interface IProductServiceClient
    {
        Task DeactivateUserProductsAsync(Guid userId, CancellationToken cancellationToken);
    }
}
